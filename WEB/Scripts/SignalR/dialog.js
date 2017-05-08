$(function () {
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

    $(window).keydown(function (e) {
        if (e.keyCode == 13)
            $("#send").click();
    });
    // Ссылка на автоматически-сгенерированный прокси хаба
    var dialog = $.connection.connectionHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    dialog.client.addMessage = function (publicId, message) {
        // Добавление сообщений на веб-страницу 
        $('#chat').append(message);
        scrollMessages();
    };
    $.connection.hub.start();
    //    .done(function () {

    //    $('#send').click(function () {
    //        // Вызываем у хаба метод Send
    //        dialog.server.send($('#recipientId').val(), $('#message').val());
    //        /*$('#body').val('');*/
    //    });


    //});
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
