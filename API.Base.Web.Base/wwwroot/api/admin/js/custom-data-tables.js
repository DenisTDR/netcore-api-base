var dataTable;
var tableElement;

function loadCustomDataTabeScripts() {
    if (typeof ignoreDataTable !== 'undefined' && ignoreDataTable) {
        return;
    }
    tableElement = $('table');
    if (!tableElement.length) {
        return;
    }
    if (tableElement.length > 0) {
        console.log('found ' + tableElement.length + ' tables.');
    }
    if (tableElement.data("tableReorder")) {
        console.log("table reorder");
        dataTable = reorder.load(tableElement);
    } else {
        bindColumnSearch(tableElement);
        dataTable = tableElement.DataTable({
            "iDisplayLength": 50,
            "lengthMenu": [[10, 25, 50, 100, 250, 500, -1], [10, 25, 50, 100, 250, 500, "All"]],
            "aaSorting": [],
            orderCellsTop: true,
            fixedHeader: true
        });
        bindColumnsHide(dataTable);
        addColumnSearchToggleButton(tableElement);
    }
    toggleFluidContainerCheck();
    $(window).resize(function () {
        toggleFluidContainerCheck();
    });
}

function toggleFluidContainerCheck() {
    var container = tableElement.closest('.container');
    if(!container.length) {
        container = tableElement.closest('.fluid-container')
    }
    var tableW = tableElement.width();
    var containerW = container.width();
    if (tableW > containerW + 50) {
        container.removeClass("container");
        container.addClass("fluid-container pl-2 pr-2");
    } else {
        container.addClass("container");
        container.removeClass("fluid-container");
        container.removeClass("pl-2");
        container.removeClass("pr-2");
    }
}

function addColumnSearchToggleButton(table) {
    var btn = $("<button class='btn btn-outline-primary mr-4'>Toggle Column Search</button>");
    btn.click(function () {
        $('.dt-column-search-row').toggle();
        toggleFluidContainerCheck();
    });
    $("#DataTables_Table_0_filter").prepend(btn);
}

function bindColumnSearch(table) {
    // Setup - add a text input to each footer cell
    table.find('thead tr').clone(true).appendTo(table.find('thead'));
    var row = table.find('thead tr:eq(1)');
    row.addClass('dt-column-search-row');
    row.hide();
    row.find('th').each(function (i) {
        var title = $(this).text().trim();
        $(this).html('<input type="text" placeholder="Search ' + title + '" />');

        $('input', this).on('keyup change', function () {
            if (dataTable.column(i).search() !== this.value) {
                dataTable
                    .column(i)
                    .search(this.value)
                    .draw();
            }
        });
    });
}

function bindColumnsHide(table) {
    $('label.toggle-vis').on( 'click', function (e) {
        e.preventDefault();
        var column = table.column( $(this).attr('data-column') );
        column.visible( ! column.visible() );
    } );
}


var reorder = {
    tableElement: null,
    load: function (tableElement) {
        this.tableElement = tableElement;
        var tableConfig = {
            "paging": false,
            "searching": false,
            rowReorder: {
                selector: 'tr'
            },
            columnDefs: [
                {targets: 0, visible: false},
                {targets: 1, orderable: false}
            ]
        };
        $("button.save-reorder").click(function () {
            reorder.saveOrder();
        });
        return tableElement.DataTable(tableConfig);
    },
    saveOrder: function () {
        var bag = {};
        var tds = this.tableElement.find("tr>td:first-child");
        var form = $(".save-reorder-form");
        form.html("");
        for (var i = 0; i < tds.length; i++) {
            var td = $(tds[i]);
            bag[td.data("orderId")] = i + 1;
            form.append("<input type='hidden' name='" + td.data("orderId") + "' value='" + (i + 1) + "' />");
        }
        form.submit();
        console.log(bag);
    }
};

$(function () {
    loadCustomDataTabeScripts();
});

