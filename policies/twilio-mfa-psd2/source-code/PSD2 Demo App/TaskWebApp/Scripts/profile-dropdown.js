var $profileDiv = $("#profile-options");
var $profileLink = $("#profile-link");
$profileLink.click(function (event) {
    event.preventDefault();
    $profileDiv.show();
});
$(document).mouseup(function (event) {
    if (!$profileDiv.is(event.target) && $profileDiv.has(event.target).length === 0)
        $profileDiv.hide();
});