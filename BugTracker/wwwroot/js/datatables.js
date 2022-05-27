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

    //const datatablesAllProjects = document.getElementById('datatablesAllProjects');
    //if (datatablesAllProjects) {
    //    new simpleDatatables.DataTable(datatablesAllProjects);
    //}
    $('#datatablesAllProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6, 7] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6, 7] }
        ]
    });

    //const datatablesArchivedProjects = document.getElementById('datatablesArchivedProjects');
    //if (datatablesArchivedProjects) {
    //    new simpleDatatables.DataTable(datatablesArchivedProjects);
    //}
    $('#datatablesArchivedProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6] }
        ]
    });

    //const datatablesUnassignedProjects = document.getElementById('datatablesUnassignedProjects');
    //if (datatablesUnassignedProjects) {
    //    new simpleDatatables.DataTable(datatablesUnassignedProjects);
    //}
    $('#datatablesUnassignedProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6, 7] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6, 7] }
        ]
    });

    const datatablesMyTickets = document.getElementById('datatablesMyTickets');
    if (datatablesMyTickets) {
        new simpleDatatables.DataTable(datatablesMyTickets);
    }

    //const datatablesAllTickets = document.getElementById('datatablesAllTickets');
    //if (datatablesAllTickets) {
    //    new simpleDatatables.DataTable(datatablesAllTickets);
    //}
    $('#datatablesAllTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] },
            { className: 'dt-body-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] }
        ]
    });

    //const datatablesArchivedTickets = document.getElementById('datatablesArchivedTickets');
    //if (datatablesArchivedTickets) {
    //    new simpleDatatables.DataTable(datatablesArchivedTickets);
    //}
    $('#datatablesArchivedTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 5, 6, 7, 8] },
            { className: 'dt-body-center', targets: [2, 3, 5, 6, 7, 8] }
        ]
    });

    //const datatablesUnassignedTickets = document.getElementById('datatablesUnassignedTickets');
    //if (datatablesUnassignedTickets) {
    //    new simpleDatatables.DataTable(datatablesUnassignedTickets);
    //}
    $('#datatablesUnassignedTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [1, 2, 3, 4, 5] },
            { className: 'dt-body-center', targets: [1, 2, 3, 4, 5] }
        ]
    });

    const datatablesPDTickets = document.getElementById('datatablesPDTickets');
    if (datatablesPDTickets) {
        new simpleDatatables.DataTable(datatablesPDTickets);
    }

});

