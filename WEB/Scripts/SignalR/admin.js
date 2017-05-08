//--------Users delete
function UserDelete(publicId) {
    var id = publicId;
    var data = { publicId: id };
    var url = "../Admin/DeleteUser";
    $.ajax({
        url: url,
        data: data,
        type: "post",
        dataType: 'html',
        success: function (data) {
            $('#delete_' + publicId).replaceWith(data);
        },
        beforeSend: function () {
            
            beforeSend('#result')
        },
        complete: function () { complete('#result') }
    });
}