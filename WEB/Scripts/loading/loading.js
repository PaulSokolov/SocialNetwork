function run_waitMe(effect, selector) {
    if ($('#' + selector).height() < 100)
        $('#' + selector).height('200px');
    $('#' + selector).waitMe({
        effect: effect,
        text: 'Please waiting...',
        bg: 'rgba(255,255,255,0.7)',
        color: '#000'
    });
}
function beforeSend(selector) {
    run_waitMe('win8', selector);
}
function complete(selector) {
    $('#'+selector).waitMe('hide');
}