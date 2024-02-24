function updateNavbarForLoggedInUser() {
    const token = localStorage.getItem('token');
    const authNavItem = document.getElementById('authNavItem');

    if (token) {
        authNavItem.innerHTML = '<a class="nav-link btn btn-danger text-white" href="#" onclick="logout()">Logout</a>';
    } else {
        authNavItem.innerHTML = '<a class="nav-link btn btn-light" href="/login.html">Login</a>';
    }
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = '/index.html';
    updateNavbarForLoggedInUser();
}

document.addEventListener('DOMContentLoaded', updateNavbarForLoggedInUser);
