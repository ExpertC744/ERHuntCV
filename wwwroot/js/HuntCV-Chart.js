/* =========================================================
   HuntCV – MASTER CHART SCRIPT
   Place at: wwwroot/js/HuntCV-Chart.js
   Needs Chart.js loaded first:
     <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

   Every  <canvas class="hc-chart"> becomes a chart. Options are data-attributes:

     data-type="line"            line (filled area) | bar          (default line)
     data-values="[3,5,4,7]"     numbers, one per label            (required)
     data-labels='["Jan","Feb"]' optional; if missing the LAST N MONTHS
                                  are generated ("May 26", "Jun 26" ...)
     data-label="No. of posts"   legend text
     data-y-title="No. of post"  y-axis title      (default = data-label)
     data-x-title="Months"       x-axis title      (default Months, "" hides)
   ========================================================= */

(function () {
    "use strict";

    var BLUE = "#2f5bea", INDIGO = "#5b3df0", GRID = "#e6ebf8",
        MUTED = "#66719a", INK = "#0b1437";

    function monthLabels(count) {
        var names = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                     "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var now = new Date(), out = [];
        for (var i = count - 1; i >= 0; i--) {
            var d = new Date(now.getFullYear(), now.getMonth() - i, 1);
            out.push(names[d.getMonth()] + " " + String(d.getFullYear()).slice(-2));
        }
        return out;
    }

    function parseJson(text, fallback) {
        if (!text) return fallback;
        try { return JSON.parse(text); } catch (e) { return fallback; }
    }

    function build(canvas) {
        var values = parseJson(canvas.getAttribute("data-values"), []);
        if (!values.length) return;

        var labels = parseJson(canvas.getAttribute("data-labels"), null) || monthLabels(values.length);
        var type = (canvas.getAttribute("data-type") || "line").toLowerCase();
        var label = canvas.getAttribute("data-label") || "Count";
        var yTitle = canvas.hasAttribute("data-y-title") ? canvas.getAttribute("data-y-title") : label;
        var xTitle = canvas.hasAttribute("data-x-title") ? canvas.getAttribute("data-x-title") : "Months";

        var dataset = (type === "bar")
            ? {
                label: label, data: values,
                backgroundColor: BLUE, hoverBackgroundColor: INDIGO,
                borderRadius: 4, categoryPercentage: .95, barPercentage: .72
            }
            : {
                label: label, data: values,
                fill: true, tension: 0, borderWidth: 2,
                borderColor: BLUE, backgroundColor: "rgba(47, 91, 234, .22)",
                pointRadius: 3.5, pointHoverRadius: 6,
                pointBackgroundColor: "#fff", pointBorderColor: INDIGO, pointBorderWidth: 2
            };

        new Chart(canvas, {
            type: type === "bar" ? "bar" : "line",
            data: { labels: labels, datasets: [dataset] },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: "top", labels: { color: INK, boxWidth: 40, boxHeight: 12, padding: 14 } },
                    tooltip: { backgroundColor: INK, padding: 10, cornerRadius: 10, titleFont: { weight: "700" } }
                },
                scales: {
                    x: { grid: { color: GRID }, title: { display: !!xTitle, text: xTitle, color: MUTED } },
                    y: {
                        beginAtZero: true, grid: { color: GRID }, ticks: { precision: 0 },
                        title: { display: !!yTitle, text: yTitle, color: MUTED }
                    }
                }
            }
        });
    }

    function boot() {
        if (typeof Chart === "undefined") {
            console.warn("HuntCV-Chart: Chart.js is not loaded.");
            return;
        }
        Chart.defaults.font.family = '"Inter", system-ui, -apple-system, "Segoe UI", sans-serif';
        Chart.defaults.color = MUTED;
        var list = document.querySelectorAll("canvas.hc-chart");
        for (var i = 0; i < list.length; i++) build(list[i]);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
