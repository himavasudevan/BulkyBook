var dataTable;


$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
        dataTable = $('#tblData').DataTable({
            "ajax": {
                "url": "/Customer/Home/GetWishList"
            },
            "columns": [
                
                {
                    "data": "product",
                    "render": function (data) {
                        return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Customer/Home/Details?productId=${data.id}">
                        ${data.title}
                        </a>
                      
					</div>
                        `
                    },
                    "width": "5%"
                },


                { "data": "product.price", "width": "25%" },
                
                {
                    "data": "product.id",
                    "render": function (data) {
                        return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Customer/Home/AddWishList?id=${data}&isWishListPage=true"
                        class="btn btn-danger mx-2"> <i class="bi bi-archive"></i></a>
                      
					</div>
                        `
                    },
                    "width": "5%"
                }
            ]
        });
    }
