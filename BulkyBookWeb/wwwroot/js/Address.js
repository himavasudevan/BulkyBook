var dataTable;


$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Customer/Home/GetAddress"
        },
        "columns": [



            { "data": "name", "width": "25%" },
            { "data": "streetAddress", "width": "25%" },
            { "data": "city", "width": "25%" },
            { "data": "state", "width": "25%" },
            { "data": "postalCode", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" },


            {
                "data": "id",
                "render": function (data) {

                    if (data == 0) {
                        return '';
                    }
                   
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Customer/Home/DeleteAddress?id=${data}"
                        class="btn btn-danger mx-2"> <i class="bi bi-archive"></i></a>
                      
					</div>
                        `
                },
                "width": "10%"
            },
             {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Customer/Home/EditAddress?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                      
					</div>
                        `
                },
                "width": "10%"
            }
             
        ]
    });
}
