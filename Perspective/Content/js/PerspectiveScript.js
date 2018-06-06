///////////////////////////
///  CURRENCY EXCHANGE  ///
///////////////////////////
// set endpoint and your access key
endpoint = 'latest'
access_key = '985e86679b7b9715037142fc2a91b00d';
// get the most recent exchange rates via the "latest" endpoint:
$.ajax({
    url: 'http://data.fixer.io/api/' + endpoint + '?access_key=' + access_key,
    dataType: 'jsonp',
    success: function (json) {

        var rate = (1 / json.rates.GBP).toFixed(2);;
        document.getElementById("xeTop").innerHTML = rate;
        document.getElementById("xeMain").innerHTML = rate;
    }
});


//////////////////////
///  PRINT BUTTON  ///
//////////////////////
function printTbl() {
    var TableToPrint = document.getElementById('tbl');
    newWin = window.open("");
    newWin.document.write(TableToPrint.outerHTML);
    newWin.print();
    newWin.close();
}


//////////////////////
///  GOOGLE CHARTS ///
//////////////////////
function showChart(totalIn, totalOut){
    google.charts.load("current", { packages: ["corechart"] });
    google.charts.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = google.visualization.arrayToDataTable([
          ['Accounts', 'Amount'],
          ['Sales', totalIn],
          ['Payments', totalOut]
        ]);

        var options = {
            backgroundColor: 'transparent',
            colors: ['#084f00', '#560105'],
            is3D: true,
        };

        var chart = new google.visualization.PieChart(document.getElementById('chart'));
        chart.draw(data, options);
    }
}