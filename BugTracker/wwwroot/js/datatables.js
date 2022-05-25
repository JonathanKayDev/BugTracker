window.addEventListener('DOMContentLoaded', event => {
    // Simple-DataTables
    // https://github.com/fiduswriter/Simple-DataTables/wiki

    const datatablesProjects = document.getElementById('datatablesProjects');
    if (datatablesProjects) {
        new simpleDatatables.DataTable(datatablesProjects);
    }

    const datatablesTickets = document.getElementById('datatablesTickets');
    if (datatablesTickets) {
        new simpleDatatables.DataTable(datatablesTickets);
    }
});
