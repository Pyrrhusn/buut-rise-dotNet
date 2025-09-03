console.log("welcome 2");
document.addEventListener("DOMContentLoaded", function () {
    function clearCookies() {
        console.log('Clearing...')
        const cookies = document.cookie.split(";");
        for (let cookie of cookies) {
            const eqPos = cookie.indexOf("=");
            const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        }
    }
    // Delay of 2 seconds before adding the event listener
    setTimeout(function () {

        // Select the logout button by class name and add click event listener
        const buttons = document.querySelectorAll(".btn");
        const logoutButton = buttons[1];

        logoutButton.addEventListener("click", function (e) {
            if (e.target.textContent === 'Logout') {
                window.location.href = "https://rise-gent2.eu.auth0.com/logout"
            }

            // Optionally redirect the user after logout
            // window.location.href = "/login"; // Update to your login page or home page
        });
    }, 2000); // 2000 milliseconds = 2 seconds
});