function getAntiforgeryToken() {
    var elements = document.getElementsByName('__RequestVerificationToken');
    if (elements.length > 0) {
        return elements[0].value;
    }
    console.error('No verification token found.');
    return null;
}