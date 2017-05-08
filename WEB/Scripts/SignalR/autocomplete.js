
$(function () {
    $("[source]").each(function () {
        var target = $(this);
        target.autocomplete({
            source: target.attr("source"),
            focus: function (event, ui) {
                $("#search").val(ui.item.name + " " + lastName);
                return false;
            },
            select: function (event, ui) {
                $("#search").val(ui.item.name + " " + ui.item.lastName);
                return false;
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>")
                .append("<a href=\"/Home/User/" + item.publicId + "\"> <div class=\"media autocomplete\"><div class=\"media-left media-top\"><img class=\"media-object autocomplete\" src=\"" + item.avatar + "\"></div><div class=\"media-body\"><h5 class=\"media-heading autocomplete\">" + item.name + " " + item.lastName + "</h5><div class=\"media\"><div class=\"media-body\"><span>" + item.address + "</span></div></div></div></div>")
                .appendTo(ul);
        };
    });
});
function autocomplete() {
    $("[source]").each(function () {
        var target = $(this);
        target.autocomplete({
            source: target.attr("source"),
            focus: function (event, ui) {
                $("#searchDelete").val(ui.item.name + " " + lastName);
                return false;
            },
            select: function (event, ui) {
                $("#searchDelete").val(ui.item.name + " " + ui.item.lastName);
                return false;
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>")
                .append("<a href=\"/Home/User/" + item.publicId + "\"> <div class=\"media autocomplete\"><div class=\"media-left media-top\"><img class=\"media-object autocomplete\" src=\"" + item.avatar + "\"></div><div class=\"media-body\"><h5 class=\"media-heading autocomplete\">" + item.name + " " + item.lastName + "</h5><div class=\"media\"><div class=\"media-body\"><span>" + item.address + "</span></div></div></div></div>")
                .appendTo(ul);
        };
    });
}