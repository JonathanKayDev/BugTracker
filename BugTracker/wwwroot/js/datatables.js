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

    const datatablesMyProjects = document.getElementById('datatablesMyProjects');
    if (datatablesMyProjects) {
        new simpleDatatables.DataTable(datatablesMyProjects);
    }

    const datatablesAllProjects = document.getElementById('datatablesAllProjects');
    if (datatablesAllProjects) {
        new simpleDatatables.DataTable(datatablesAllProjects);
    }

    const datatablesArchivedProjects = document.getElementById('datatablesArchivedProjects');
    if (datatablesArchivedProjects) {
        new simpleDatatables.DataTable(datatablesArchivedProjects);
    }

    const datatablesMyTickets = document.getElementById('datatablesMyTickets');
    if (datatablesMyTickets) {
        new simpleDatatables.DataTable(datatablesMyTickets);
    }

    const datatablesAllTickets = document.getElementById('datatablesAllTickets');
    if (datatablesAllTickets) {
        new simpleDatatables.DataTable(datatablesAllTickets);
    }

    const datatablesArchivedTickets = document.getElementById('datatablesArchivedTickets');
    if (datatablesArchivedTickets) {
        new simpleDatatables.DataTable(datatablesArchivedTickets);
    }

    const datatablesUnassignedTickets = document.getElementById('datatablesUnassignedTickets');
    if (datatablesUnassignedTickets) {
        new simpleDatatables.DataTable(datatablesUnassignedTickets);
    }

});
