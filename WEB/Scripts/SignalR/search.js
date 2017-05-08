
function filterAges(fromAge, toAgeList, toAge = null, fromAgeList = null) {
    if (toAge != null) {
        fromAgeList.each(function (i) {
            if ($(this).val() == "default")
                $(this).show();
            else if (toAge < $(this).val())
                $(this).hide();
            else
                $(this).show();
        });
    }
    else {
        toAgeList.each(function (i) {

            if ($(this).val() == "default")
                $(this).show();
            else if (fromAge > $(this).val())
                $(this).hide();
            else
                $(this).show();
        });
    }
}
$(function () {
    $("#cityLabel").hide();
    $("#cities").hide();
    var docHeight = $(window).height();
    var footerHeight = $('footer').height();
    var footerTop = $('footer').position().top + footerHeight;

    //if (footerTop < docHeight) {
    //    $('footer').css('margin-top', 10 + (docHeight - footerTop) + 'px');

});
$("#searchButton").click(function (e) {
    e.preventDefault();
    var data = getSearchData();
    searchRequest(data);
})
$('#sort').on("change", function () {   
    var data = getSearchData();
    searchRequest(data);
});
$('#countries').on("change", function () {
    if ($(this).val() == "default") {
        $("#cityLabel").hide();
        $("#cities").hide();
    }
    else {
        $("#cityLabel").show();
        $("#cities").show();
        $.ajax({
            type: "GET",
            url: "/Home/Cities",
            data: { countryId: $('#countries option:selected').val() },
            dataType: "html",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#cities").empty().prepend(data);
            },
            beforeSend: function () { beforeSend('result') },
            complete: function () { complete('result') },
            failure: function () {
                alert("error");
            }
        });
    }
    var data = getSearchData();
    searchRequest(data);
});
$('#cities').on("change", function () {
    var data = getSearchData();
    searchRequest(data);
});
$('#from').on("change", function () {
    var options = $('#to').children();
    var fromSelectedAge = $(this).val();
    filterAges(fromSelectedAge, options);
    var data = getSearchData();
    searchRequest(data);
});
$('#to').on("change", function () {
    var options = $('#from').children();
    var toSelectedAge = $(this).val();
    filterAges(null, null, toSelectedAge, options);
    var data = getSearchData();
    searchRequest(data);
});
$('#gender').on("change", function () {
    var data = getSearchData();
    searchRequest(data);
});
function updateQueryString(data) {
    
    var query = "?";
    if (data.ageFrom !== "default")
        query = query + "&ageFrom=" + data.ageFrom;
    if (data.ageTo !== "default")
        query = query + "&ageTo=" + data.ageTo;
    if (data.cityId !== "default")
        query = query + "&cityId=" + data.cityId;
    if (data.countryId !== "default")
        query = query + "&countryId=" + data.countryId;
    if (data.sex !== "default")
        query = query + "&sex=" + data.sex;
    if (data.sort !== "default")
        query = query + "&sort=" + data.sort;
    if (data.aboutConcurence !== "")
        query = query + "&aboutConcurence=" + data.aboutConcurence;
    if (data.activityConcurence !== "")
        query = query + "&activityConcurence=" + data.activityConcurence;
    //var stateObj = { queryHistory: query };
    //window.history.pushState(stateObj, "Search", query);
    //document.location.href = window.location.origin + "/Users/Index?search=" + query;
}
function searchRequest(data) {
    $.ajax({
        type: "POST",
        url: "/Users/Search",
        data: JSON.stringify(data),
        dataType: "html",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#result").empty().prepend(data);
        },
        beforeSend: function () { beforeSend('result') },
        complete: function () { complete('result') },
        failure: function () {
            alert("error");
        }
    })
}
function getSearchData() {
    var data = {
        ageFrom: $("#from").val(),
        ageTo: $("#to").val(),
        cityId: $("#cities").val(),
        countryId: $("#countries").val(),
        sex: $("#gender").val(),
        sort: $("#sort").val(),
        activityConcurence: $("#search").val(),
        aboutConcurence: $("#search").val(),
        search: $("#search").val()
    };
    return data;
}
//$(function () {
//    $("#CountryId").change(function () {
//        $.ajax({
//            type: "GET",
//            url: "/Home/Cities",
//            data: { countryId: $('#CountryId option:selected').val() },
//            dataType: "html",
//            contentType: "application/json; charset=utf-8",
//            success: function (data) {
//                $("#CityId").empty().prepend(data);
//            },
//            beforeSend: function () { beforeSend('result') },
//            complete: function () { complete('result') },
//            failure: function () {
//                alert("error");
//            }
//        });
//    });
//})