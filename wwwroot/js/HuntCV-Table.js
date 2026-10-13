/* =========================================================
   HuntCV – MASTER TABLE SCRIPT  (plain JavaScript)
   Place at: wwwroot/js/HuntCV-Table.js
   No jQuery / DataTables needed. Every <table class="hc-datatable">
   automatically gets:
     - "entries per page" dropdown  (10, 25, 50, 100)
     - live search box
     - click-to-sort column headers
     - "Showing x to y of z entries" + pagination

   Optional attributes on the <table>:
     data-page-length="25"   default rows per page (10, 25, 50 or 100)
     data-no-sort="5,6"      column indexes that can't be sorted
     data-order-col="3"      column to sort by on load (optional)
     data-order-dir="desc"   asc | desc (optional)
     Dates like "12 Aug 2026" and "12-08-2026" sort as real dates.
   ========================================================= */

(function () {
    "use strict";

    var LENGTHS = [10, 25, 50, 100];

    function el(tag, cls, html) {
        var e = document.createElement(tag);
        if (cls) e.className = cls;
        if (html !== undefined) e.innerHTML = html;
        return e;
    }

    var MONTHS = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];

    /* "12 Aug 2026" or "12-08-2026" -> timestamp, otherwise null */
    function toTime(s) {
        var m = s.match(/^(\d{1,2})[ -]([A-Za-z]{3})[A-Za-z]*[ ,-]+(\d{4})$/);
        if (m) {
            var mo = MONTHS.indexOf(m[2].toLowerCase());
            if (mo > -1) return new Date(+m[3], mo, +m[1]).getTime();
        }
        m = s.match(/^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/);
        if (m) return new Date(+m[3], +m[2] - 1, +m[1]).getTime();
        return null;
    }

    function cellValue(row, col) {
        var c = row.cells[col];
        return c ? c.textContent.replace(/\s+/g, " ").trim() : "";
    }

    function init(table) {
        var tbody = table.tBodies[0];
        if (!tbody) return;

        var allRows = Array.prototype.slice.call(tbody.rows);
        var headers = Array.prototype.slice.call(table.tHead.rows[0].cells);
        var colCount = headers.length;

        var noSort = String(table.getAttribute("data-no-sort") || "")
            .split(",").filter(Boolean).map(Number);

        var state = {
            page: 1,
            length: parseInt(table.getAttribute("data-page-length"), 10) || 25,
            query: "",
            sortCol: -1,
            sortDir: "asc"
        };
        if (LENGTHS.indexOf(state.length) === -1) state.length = 25;

        /* optional starting sort:  data-order-col="3" data-order-dir="desc" */
        var oc = parseInt(table.getAttribute("data-order-col"), 10);
        if (!isNaN(oc) && oc >= 0 && oc < colCount && noSort.indexOf(oc) === -1) {
            state.sortCol = oc;
            state.sortDir = (table.getAttribute("data-order-dir") || "asc").toLowerCase() === "desc" ? "desc" : "asc";
        }

        /* pre-compute searchable text once */
        var searchText = allRows.map(function (r) {
            return r.textContent.replace(/\s+/g, " ").toLowerCase();
        });
        var rowObjs = allRows.map(function (r, i) { return { row: r, text: searchText[i], idx: i }; });

        /* ---------- build wrapper ---------- */
        /* the scroll box holds ONLY the table, so search / entries / paging stay in place */
        var scroll = table.closest(".hc-table-scroll");
        if (!scroll) {
            scroll = el("div", "hc-table-scroll");
            table.parentNode.insertBefore(scroll, table);
            scroll.appendChild(table);
        }

        var wrapper = el("div", "dataTables_wrapper");
        scroll.parentNode.insertBefore(wrapper, scroll);

        /* top bar */
        var top = el("div", "row mb-3 align-items-center");
        var colL = el("div", "col-sm-6");
        var colR = el("div", "col-sm-6 text-sm-end mt-2 mt-sm-0");

        var lenBox = el("div", "dataTables_length");
        var lenLabel = el("label");
        var select = el("select", "form-select form-select-sm");
        LENGTHS.forEach(function (n) {
            var o = el("option", "", n);
            o.value = n;
            if (n === state.length) o.selected = true;
            select.appendChild(o);
        });
        lenLabel.appendChild(select);
        lenLabel.appendChild(document.createTextNode(" entries per page"));
        lenBox.appendChild(lenLabel);
        colL.appendChild(lenBox);

        var filBox = el("div", "dataTables_filter");
        var filLabel = el("label");
        var input = el("input", "form-control");
        input.type = "search";
        input.placeholder = "Search...";
        input.setAttribute("aria-label", "Search table");
        filLabel.appendChild(input);
        filBox.appendChild(filLabel);
        colR.appendChild(filBox);

        top.appendChild(colL);
        top.appendChild(colR);

        /* bottom bar */
        var bottom = el("div", "row");
        var colInfo = el("div", "col-sm-5");
        var colPage = el("div", "col-sm-7 mt-2 mt-sm-0");
        var info = el("div", "dataTables_info");
        var pager = el("ul", "pagination justify-content-sm-end mb-0");
        colInfo.appendChild(info);
        colPage.appendChild(pager);
        bottom.appendChild(colInfo);
        bottom.appendChild(colPage);

        wrapper.appendChild(top);
        wrapper.appendChild(scroll);
        wrapper.appendChild(bottom);

        /* ---------- sorting setup ---------- */
        headers.forEach(function (th, i) {
            if (noSort.indexOf(i) !== -1) return;
            th.classList.add("sorting");
            th.setAttribute("tabindex", "0");
            function go() {
                if (state.sortCol === i) {
                    state.sortDir = state.sortDir === "asc" ? "desc" : "asc";
                } else {
                    state.sortCol = i;
                    state.sortDir = "asc";
                }
                state.page = 1;
                render();
            }
            th.addEventListener("click", go);
            th.addEventListener("keydown", function (e) {
                if (e.key === "Enter" || e.key === " ") { e.preventDefault(); go(); }
            });
        });

        function compare(a, b) {
            var x = cellValue(a.row, state.sortCol);
            var y = cellValue(b.row, state.sortCol);
            var nx = parseFloat(x.replace(/,/g, ""));
            var ny = parseFloat(y.replace(/,/g, ""));
            var r;
            var tx = toTime(x), ty = toTime(y);
            if (tx !== null && ty !== null) {
                r = tx - ty;
            } else if (!isNaN(nx) && !isNaN(ny) && /^[\d.,\s-]+$/.test(x) && /^[\d.,\s-]+$/.test(y)) {
                r = nx - ny;
            } else {
                r = x.localeCompare(y, undefined, { sensitivity: "base", numeric: true });
            }
            if (r === 0) r = a.idx - b.idx; /* stable */
            return state.sortDir === "asc" ? r : -r;
        }

        /* ---------- pagination buttons ---------- */
        function pageItem(label, page, disabled, active) {
            var li = el("li", "page-item" + (disabled ? " disabled" : "") + (active ? " active" : ""));
            var a = el("a", "page-link", label);
            a.href = "#";
            a.addEventListener("click", function (e) {
                e.preventDefault();
                if (disabled || active) return;
                state.page = page;
                render();
            });
            li.appendChild(a);
            return li;
        }

        function pageNumbers(current, total) {
            var out = [], i;
            if (total <= 7) {
                for (i = 1; i <= total; i++) out.push(i);
                return out;
            }
            out.push(1);
            if (current > 3) out.push("…");
            for (i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) out.push(i);
            if (current < total - 2) out.push("…");
            out.push(total);
            return out;
        }

        /* ---------- render ---------- */
        function render() {
            var q = state.query.trim().toLowerCase();
            var words = q ? q.split(/\s+/) : [];

            var list = rowObjs.filter(function (o) {
                return words.every(function (w) { return o.text.indexOf(w) !== -1; });
            });

            if (state.sortCol > -1) list.sort(compare);

            var total = list.length;
            var pages = Math.max(1, Math.ceil(total / state.length));
            if (state.page > pages) state.page = pages;

            var start = (state.page - 1) * state.length;
            var end = Math.min(start + state.length, total);

            /* rows */
            tbody.innerHTML = "";
            if (total === 0) {
                var tr = el("tr");
                var td = el("td", "dataTables_empty", "No matching records found");
                td.colSpan = colCount;
                tr.appendChild(td);
                tbody.appendChild(tr);
            } else {
                list.slice(start, end).forEach(function (o) { tbody.appendChild(o.row); });
            }

            /* header arrows */
            headers.forEach(function (th, i) {
                if (noSort.indexOf(i) !== -1) return;
                th.classList.remove("sorting_asc", "sorting_desc");
                th.classList.add("sorting");
                if (i === state.sortCol) {
                    th.classList.remove("sorting");
                    th.classList.add(state.sortDir === "asc" ? "sorting_asc" : "sorting_desc");
                }
            });

            /* info */
            var totalAll = rowObjs.length;
            if (total === 0) {
                info.textContent = "Showing 0 to 0 of 0 entries";
            } else {
                info.textContent = "Showing " + (start + 1) + " to " + end + " of " + total + " entries" +
                    (total !== totalAll ? " (filtered from " + totalAll + " total entries)" : "");
            }

            /* pager */
            pager.innerHTML = "";
            pager.appendChild(pageItem("<", state.page - 1, state.page === 1, false));
            pageNumbers(state.page, pages).forEach(function (p) {
                if (p === "…") {
                    pager.appendChild(pageItem("…", 0, true, false));
                } else {
                    pager.appendChild(pageItem(p, p, false, p === state.page));
                }
            });
            pager.appendChild(pageItem(">", state.page + 1, state.page === pages, false));
        }

        /* ---------- events ---------- */
        select.addEventListener("change", function () {
            state.length = parseInt(select.value, 10);
            state.page = 1;
            render();
        });

        var timer;
        input.addEventListener("input", function () {
            clearTimeout(timer);
            timer = setTimeout(function () {
                state.query = input.value;
                state.page = 1;
                render();
            }, 120);
        });

        render();
    }

    function boot() {
        var tables = document.querySelectorAll("table.hc-datatable");
        for (var i = 0; i < tables.length; i++) init(tables[i]);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();