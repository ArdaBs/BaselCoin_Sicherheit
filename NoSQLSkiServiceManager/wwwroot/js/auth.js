let logoutTimer;
let isTimeoutLogout = false; 

function startLogoutTimer() {
    const token = localStorage.getItem('token');
    if (!token) {
        return;
    }

    const logoutTime = 0.5 * 60 * 1000; // 30 Sekunden
    if (logoutTimer) clearTimeout(logoutTimer);
    logoutTimer = setTimeout(() => {
        isTimeoutLogout = true;
        logout();
    }, logoutTime);
}


function logout() {
    if (isTimeoutLogout) {
        var sessionTimeoutModal = new bootstrap.Modal(document.getElementById('sessionTimeoutModal'));
        sessionTimeoutModal.show();

        $('#sessionTimeoutModal').on('hidden.bs.modal', function () {
            performLogoutActions();
        });
    } else {
        performLogoutActions();
    }
}

function performLogoutActions() {
    localStorage.removeItem('token');
    window.location.href = '/index.html';
    updateNavbarForLoggedInUser();
}
function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
        return null;
    }
}

function updateNavbarForLoggedInUser() {
    const token = localStorage.getItem('token');
    const authNavItem = document.getElementById('authNavItem');
    const navbarNav = document.getElementById('navbarNav').getElementsByClassName('navbar-nav')[1];

    // Bereinigt dynamisch hinzugefügte Nav-Items
    $(".dynamic-nav").remove(); // Verwendet jQuery, um Elemente mit der Klasse 'dynamic-nav' zu entfernen

    if (token) {
        const decodedToken = parseJwt(token);
        const userRole = decodedToken?.role;

        // Erstellt dynamisch den Account-Dashboard-Link
        const accountDashboardLink = $('<li class="nav-item dynamic-nav"><a class="nav-link" href="/account-dashboard.html">Account Dashboard</a></li>');

        // Überprüft, ob der Benutzer die Admin-Rolle hat
        if (userRole === 'Admin') {
            // Erstellt den Admin-Dashboard-Link
            const adminDashboardLink = $('<li class="nav-item dynamic-nav"><a class="nav-link" href="/admin-dashboard.html">Admin Dashboard</a></li>');
            $(navbarNav).append(adminDashboardLink).append(accountDashboardLink);
        } else {
            // Fügt nur den Account-Dashboard-Link hinzu
            $(navbarNav).append(accountDashboardLink);
        }

        // Setzt den Logout-Button
        authNavItem.innerHTML = '<a class="nav-link btn btn-danger text-white" href="#" onclick="manualLogout()">Logout</a>';
    } else {
        // Zeigt den Login-Button, wenn der Benutzer nicht eingeloggt ist
        authNavItem.innerHTML = '<a class="nav-link btn btn-light text-dark" href="/login.html">Login</a>';
    }
}

function manualLogout() {
    localStorage.removeItem('token');
    window.location.href = '/index.html';
    updateNavbarForLoggedInUser();
}

document.addEventListener('DOMContentLoaded', () => {
    updateNavbarForLoggedInUser();
});

const activityEvents = ['mousemove', 'keydown', 'scroll', 'touchstart'];
activityEvents.forEach(event => {
    window.addEventListener(event, startLogoutTimer);
});
