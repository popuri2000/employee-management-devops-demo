(function () {
    var state = {
        searchTerm: '',
        department: '',
        pageNumber: 1,
        pageSize: 10
    };
    var pendingDeleteId = null;

    function escapeHtml(value) {
        return $('<div>').text(value == null ? '' : value).html();
    }

    function formatCurrency(value) {
        return Number(value).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
    }

    function formatDate(value) {
        var d = new Date(value);
        return isNaN(d.getTime()) ? '' : d.toLocaleDateString();
    }

    function renderRows(items) {
        var $body = $('#employeeTableBody').empty();

        if (!items || items.length === 0) {
            $body.append('<tr><td colspan="8" class="text-center text-muted py-4">No employees found.</td></tr>');
            return;
        }

        items.forEach(function (emp) {
            var statusBadge = emp.isActive
                ? '<span class="badge text-bg-success">Active</span>'
                : '<span class="badge text-bg-secondary">Inactive</span>';

            var row =
                '<tr>' +
                '<td>' + escapeHtml(emp.name) + '</td>' +
                '<td>' + escapeHtml(emp.email) + '</td>' +
                '<td>' + escapeHtml(emp.department) + '</td>' +
                '<td>' + escapeHtml(emp.designation) + '</td>' +
                '<td>' + formatCurrency(emp.salary) + '</td>' +
                '<td>' + formatDate(emp.joiningDate) + '</td>' +
                '<td>' + statusBadge + '</td>' +
                '<td class="text-end">' +
                '<a href="/Employees/Edit/' + emp.id + '" class="btn btn-sm btn-outline-primary me-1">Edit</a>' +
                '<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="' + emp.id + '" data-name="' + escapeHtml(emp.name) + '">Delete</button>' +
                '</td>' +
                '</tr>';

            $body.append(row);
        });
    }

    function renderPagination(result) {
        var $pagination = $('#pagination').empty();
        var totalPages = result.totalPages || 0;

        $('#resultSummary').text(
            result.totalCount === 0
                ? 'No results'
                : 'Showing page ' + result.pageNumber + ' of ' + totalPages + ' (' + result.totalCount + ' total employees)'
        );

        if (totalPages <= 1) {
            return;
        }

        function pageItem(label, page, disabled, active) {
            return '<li class="page-item' + (disabled ? ' disabled' : '') + (active ? ' active' : '') + '">' +
                '<a class="page-link" href="#" data-page="' + page + '">' + label + '</a></li>';
        }

        $pagination.append(pageItem('Previous', result.pageNumber - 1, !result.hasPreviousPage, false));

        for (var p = 1; p <= totalPages; p++) {
            $pagination.append(pageItem(p, p, false, p === result.pageNumber));
        }

        $pagination.append(pageItem('Next', result.pageNumber + 1, !result.hasNextPage, false));
    }

    function loadEmployees() {
        $.ajax({
            url: '/Employees/List',
            method: 'GET',
            data: {
                searchTerm: state.searchTerm,
                department: state.department,
                pageNumber: state.pageNumber,
                pageSize: state.pageSize
            }
        }).done(function (result) {
            renderRows(result.items);
            renderPagination(result);
        }).fail(function () {
            showToast('Unable to load employees from the API.', 'danger');
            $('#employeeTableBody').html('<tr><td colspan="8" class="text-center text-danger py-4">Failed to load data.</td></tr>');
        });
    }

    var searchDebounce;
    $('#searchInput').on('input', function () {
        clearTimeout(searchDebounce);
        var value = $(this).val();
        searchDebounce = setTimeout(function () {
            state.searchTerm = value;
            state.pageNumber = 1;
            loadEmployees();
        }, 350);
    });

    $('#departmentFilter').on('change', function () {
        state.department = $(this).val();
        state.pageNumber = 1;
        loadEmployees();
    });

    $('#pageSizeSelect').on('change', function () {
        state.pageSize = parseInt($(this).val(), 10);
        state.pageNumber = 1;
        loadEmployees();
    });

    $('#pagination').on('click', 'a.page-link', function (e) {
        e.preventDefault();
        var page = parseInt($(this).data('page'), 10);
        if (!page || page < 1) return;
        state.pageNumber = page;
        loadEmployees();
    });

    $('#employeeTableBody').on('click', '.btn-delete', function () {
        pendingDeleteId = $(this).data('id');
        $('#deleteEmployeeName').text($(this).data('name'));
        var modal = new bootstrap.Modal(document.getElementById('deleteModal'));
        modal.show();
    });

    $('#confirmDeleteBtn').on('click', function () {
        if (!pendingDeleteId) return;

        $.ajax({
            url: '/Employees/Delete/' + pendingDeleteId,
            method: 'POST'
        }).done(function (response) {
            bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
            showToast(response.message || 'Employee deleted.', 'success');
            loadEmployees();
        }).fail(function (xhr) {
            bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
            var message = (xhr.responseJSON && xhr.responseJSON.message) || 'Failed to delete employee.';
            showToast(message, 'danger');
        }).always(function () {
            pendingDeleteId = null;
        });
    });

    loadEmployees();
})();
