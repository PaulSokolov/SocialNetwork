$(function () {
    var dialog = $.connection.connectionHub;

    dialog.client.addMessage = function (message) {
        $("#chat").append(message);
        scrollMessages();
        subscribeOnHoverEvents();
    };
    dialog.client.readMessage = function (messageId) {
        $("#mes_" + messageId).removeClass("unread");
    };
    $.connection.hub.start();

    updateContainer();
    scrollMessages();

    $(window).resize(function () {
        updateContainer();
    });
    $("#send").click(function (e) {
        e.preventDefault();
        if ($("#sendForm").children("textarea").val() === "")
            e.preventDefault();
        else {
            $.ajax({
                url: $('#sendForm').attr('action'),
                data: $('#sendForm').serialize(),
                type: "post",
                dataType: 'html',
                success: function (data) {
                    $("#chat").append(data);
                    $("#sendForm").children("textarea").val("");
                    scrollMessages();
                }
            });
        }
    });
    subscribeOnHoverEvents();
    

    $(window).keydown(function (e) {
        if (e.keyCode == 13)
            $("#send").click();
    });
    
});
// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
function updateContainer() {
    var $containerHeight = $(window).height();
    //var footerHeight = $('footer').height();
    //var footerTop = $('footer').position().top + footerHeight;

    //if (footerTop < $containerHeight) {
    //    $('footer').css('margin-top', 100 + (docHeight - footerTop) + 'px');
    $('.height').css({
        height: $containerHeight - 300
    });

}
function scrollMessages() {
    var chat = document.getElementById("chat");
    chat.scrollTop = chat.scrollHeight;
}
function subscribeOnHoverEvents() {
    $(".message.dialog_last_message").mouseenter(function () {
        if (this.getAttribute('name') === 'fromMe') return;
        if ($(this).hasClass("unread")) {
            var mesId = this.getAttribute("id").split("_")[1];
            $.ajax({
                url: "/Messages/Read",
                data: { messageId: mesId },
                type: "post",
                dataType: "json",
                success: function (data) {
                    if (data.isRead === true) {
                        $('#mes_' + mesId).removeClass("unread");
                    }
                }
            });
        }
    });
}
