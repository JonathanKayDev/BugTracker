window.addEventListener('DOMContentLoaded', event => {
    // Simple-DataTables
    // https://github.com/fiduswriter/Simple-DataTables/wiki

    $('#datatablesProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [1, 2, 3, 4] },
            { className: 'dt-body-center', targets: [1, 2, 3, 4] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 5
    });

    $('#datatablesTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [3, 4, 5] },
            { className: 'dt-body-center', targets: [3, 4, 5] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 5
    });

    $('#datatablesMyProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6, 7] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6, 7] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesAllProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6, 7] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6, 7] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesArchivedProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesUnassignedProjects').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 5, 6, 7] },
            { className: 'dt-body-center', targets: [2, 3, 4, 5, 6, 7] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesMyTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] },
            { className: 'dt-body-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesAllTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] },
            { className: 'dt-body-center', targets: [2, 3, 4, 6, 7, 8, 9, 10, 11] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesArchivedTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [2, 3, 5, 6, 7, 8] },
            { className: 'dt-body-center', targets: [2, 3, 5, 6, 7, 8] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesUnassignedTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [1, 2, 3, 4, 5] },
            { className: 'dt-body-center', targets: [1, 2, 3, 4, 5] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesPDTickets').DataTable({
        columnDefs: [
            { className: 'dt-head-center', targets: [3, 4, 5] },
            { className: 'dt-body-center', targets: [3, 4, 5] }
        ],
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });

    $('#datatablesCompanyMembers').DataTable({
        language: {
            lengthMenu: "_MENU_",
            search: "_INPUT_",
            searchPlaceholder: "Search",
            info: "_START_ to _END_ of _TOTAL_ entries",
            "paginate": {
                "previous": "<",
                "next": ">"
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        iDisplayLength: 10
    });
});

