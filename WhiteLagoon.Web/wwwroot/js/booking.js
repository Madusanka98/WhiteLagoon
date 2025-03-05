var dataTable;

$(document).ready(function () {
    const urlParms = new URLSearchParams(window.location.search);
    const status = urlParms.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    tblBookings = $('#tblBookings').dataTable({
        "ajax": {
            url: 'booking/getall?status=' + status
        },
        "columns": [
            { data: 'id',"width":"5%"},
            { data: 'name',"width":"15%"},
            { data: 'phone',"width":"10%"},
            { data: 'email',"width":"15%"},
            { data: 'status',"width":"10%"},
            { data: 'checkInDate',"width":"10%"},
            { data: 'nights',"width":"10%"},
            { data: 'totalCost',render:$.fn.dataTable.render.number(',','.',2),"width":"10%"},
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                    <a href="/booking/bookingDetails?bookingId=${data}" class="btn btn-outline-warning ms-2"><i class="bi bi-pencile-square"></i>Details</a></div>`
                }
            }
        ]
    })

}