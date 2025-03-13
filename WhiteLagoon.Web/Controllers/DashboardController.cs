using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month ==1 ? 12:DateTime.Now.Month - 1;
        private DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        private DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {

            var totalBooking = _unitOfWork.booking.GetAll(u=>u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);
            var countByCurrentMonth = totalBooking.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBooking.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);

            return Json(GetRadialBarChartVM(totalBooking.Count(), countByCurrentMonth, countByPreviousMonth));
        }

        public async Task<IActionResult> GetRegisteredUserChartData()
        {

            var totalUsers = _unitOfWork.user.GetAll();
            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate && u.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialBarChartVM(totalUsers.Count(),countByCurrentMonth,countByPreviousMonth));
        }

        public async Task<IActionResult> GetRevenueChartData()
        {
            var totalBooking = _unitOfWork.booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBooking.Sum(u => u.TotalCost));

            var countByCurrentMonth = totalBooking.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now).Sum(u=>u.TotalCost);

            var countByPreviousMonth = totalBooking.Where(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

            return Json(GetRadialBarChartVM(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }

        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBooking = _unitOfWork.booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

            var customerWithOneBooking = totalBooking.GroupBy(b=>b.UserId).Where(g=>g.Count() == 1).Select(x=>x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();
            int bookingsByReturningCustomer = totalBooking.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVM = new()
            {
                labels = new string[] { "New Customer Booking", "Returning Customer Booking" },
                series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return Json(pieChartVM);
        }

        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData = _unitOfWork.booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
             u.BookingDate.Date <= DateTime.Now)
                 .GroupBy(b => b.BookingDate.Date)
                 .Select(u => new {
                     DateTime = (DateTime?)u.Key, // Make DateTime nullable
                     NewBookingCount = u.Count()
                 });

            var customerData = _unitOfWork.user.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) &&
            u.CreatedAt <= DateTime.Now)
                .GroupBy(b => b.CreatedAt) // Group by Date only
                .Select(u => new {
                    DateTime = (DateTime?)u.Key, // Make DateTime nullable
                    NewCustomerCount = u.Count()
                });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

            // Ensure both collections have the same anonymous type
            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime?.ToString("MM/dd/yyyy")).ToArray(); // Use nullable DateTime

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data = newBookingData
                },
                new ChartData
                {
                    Name = "New Members",
                    Data = newCustomerData
                },
            };

            LineChartVM LineChartDto = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return Ok(LineChartDto); // Return the result with Ok()
        }

        private static RadialBarChartVM GetRadialBarChartVM(int totalCount, double currentMonthCount, double prevMonthCount )
        {
            RadialBarChartVM radialBarChartVM = new();

            int increasedDecreaseRation = 100;
            if (currentMonthCount != 0)
            {
                increasedDecreaseRation = Convert.ToInt32(((currentMonthCount - prevMonthCount) / totalCount) * 100);
            }

            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            radialBarChartVM.HasRatioIncreased = currentMonthCount > prevMonthCount;
            radialBarChartVM.Series = new int[] { increasedDecreaseRation };

            return radialBarChartVM;
        }
    }
}
