
// toggle edit functionality for the profile page
function toggleEdit() {
    var form = document.getElementById("profileForm")
    var inputs = form.getElementsByTagName("input");

    for (var i = 0; i < inputs.length; i++) {
        inputs[i].readOnly = !inputs[i].readOnly;
    }

    // hide edit button and show confirm details button
    document.getElementById("confirm").removeAttribute("hidden");
    document.getElementById("editButton").setAttribute("hidden", true);
}