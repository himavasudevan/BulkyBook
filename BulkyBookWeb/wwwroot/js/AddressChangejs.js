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
            {
                "data": null,
                "width": "5%",
                "render": function (data, type, row, meta) {
                    // Create a radio button with a unique ID for each row
                    return '<input type="radio" name="addressRadio" value="' + row.id + '">';
                }
            },




            { "data": "name", "width": "25%" },
            { "data": "streetAddress", "width": "25%" },
            { "data": "city", "width": "25%" },
            { "data": "state", "width": "25%" },
            { "data": "postalCode", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" }


            
     

        ]
    });
}
