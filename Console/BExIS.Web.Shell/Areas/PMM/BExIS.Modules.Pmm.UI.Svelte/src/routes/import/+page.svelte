<script>
  import Papa from 'papaparse';
	import { split } from 'postcss/lib/list';
  let csvData = '';
  let jsonData = []; 
  let file; 
  let downloadLink = '';

  let validData = [];
  let removedData = [];
  let downloadLinkValidData = '';
  let downloadLinkRemovedData = '';
  let Error = '';


  function handleFileUpload(event) {
    file = event.target.files[0]; 
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        csvData = reader.result; 
        parseCSVToJson(csvData); 
      };
      reader.readAsText(file);
    }
  }

  function countCommas(data) {
    return data.split(',').length - 1;
  }

  function sortData(data) {
    validData = [];
    removedData = [];
    Error = '';

    data.forEach(row => {
      const dataURL = row['Data URL'];
      // Überprüfe, ob dataURL leer, undefined oder null ist
      if (dataURL) {
        const commaDataURL = countCommas(dataURL);

        const dataPresent = row['Data present'];
        const commaDataPresent = countCommas(dataPresent);

        const dataLicense = row['Data License'];
        const commaDataLicense = countCommas(dataLicense);

        const dataPublisher = row['Data Publisher'];
        const commaDataPublisher = countCommas(dataPublisher);

        const dataFormat = row['Data format'];
        const commaDataFormat = countCommas(dataFormat);

        const dataURLResolves = row['Data URL resolves'];
        const commaDataURLResolves = countCommas(dataURLResolves);

        const dataDOI = row['Data DOI'];
        const commaDataDOI = countCommas(dataDOI);

        const dataDOIResolves = row['Data DOI resolves'];
        const commaDataDOIResolves = countCommas(dataDOIResolves);

        const codePresent = row['Code present'];
        const commaCodePresent = countCommas(codePresent);

        const codeLicense = row['Code License'];
        const commaCodeLicense = countCommas(codeLicense);

        const codePublisher = row['Code Publisher'];
        const commaCodePublisher = countCommas(codePublisher);

        const codeFormat = row['Code format'];
        const commaCodeFormat = countCommas(codeFormat);

        const codeURL = row['Code URL'];
        const commaCodeURL = countCommas(codeURL);

        const codeURLResolves = row['Code URL resolves'];
        const commaCodeURLResolves = countCommas(codeURLResolves);

        const codeDOI = row['Code DOI'];
        const commaCodeDOI = countCommas(codeDOI);
    
        const codeDOIResolves = row['Code DOI resolves'];
        const commaCodeDOIResolves = countCommas(codeDOIResolves);
  
          if (commaDataURL == 0
            ||
            commaDataURL === commaDataPresent &&
            commaDataPresent === commaDataLicense &&
            commaDataLicense === commaDataPublisher &&
            commaDataPublisher === commaDataFormat &&
            commaDataFormat === commaDataURLResolves &&
            commaDataURLResolves === commaDataDOI &&
            commaDataDOI === commaDataDOIResolves &&
            commaDataDOIResolves === commaCodePresent &&
            commaCodePresent === commaCodeLicense &&
            commaCodeLicense === commaCodePublisher &&
            commaCodePublisher === commaCodeFormat &&
            commaCodeFormat === commaCodeURL &&
            commaCodeURL === commaCodeURLResolves &&
            commaCodeURLResolves === commaCodeDOI &&
            commaCodeDOI === commaCodeDOIResolves 
          ) {
            validData.push(row);
          } else { removedData.push(row); }

      } else { Error = 'Array ist leer oder nicht definiert';}
    });
    createDownloadLinks();
  }

  function parseCSVToJson(data) {
    Papa.parse(data, {
      header: true, 
      complete: function(results) {
        jsonData = results.data; 
        sortData(jsonData);
        createDownloadLinks();
      }
    });
  }

  function createDownloadLinks() {
    const jsonString = JSON.stringify(jsonData, null, 2); 
    const blob = new Blob([jsonString], { type: 'application/json' }); 
    const url = URL.createObjectURL(blob); 
    downloadLink = url; 

    const validCSVString = Papa.unparse(validData, { delimiter: ',' }); // Ensure comma as separator
    const blobValid = new Blob([validCSVString], { type: 'text/csv' });
    downloadLinkValidData = URL.createObjectURL(blobValid);

    const removedCSVString = Papa.unparse(removedData, { delimiter: ',' }); // Ensure comma as separator
    const blobRemoved = new Blob([removedCSVString], { type: 'text/csv' });
    downloadLinkRemovedData = URL.createObjectURL(blobRemoved);
  }

  function downloadValidData() {
      const link = document.createElement('a');
      link.href = downloadLinkValidData;
      link.download = 'valid__data.csv';
      link.click();
    }

    function downloadRemovedData() {
      const link = document.createElement('a');
      link.href = downloadLinkRemovedData;
      link.download = 'removed__data.csv';
      link.click();
    }

  function downloadJsonFile() {
    const link = document.createElement('a');
    link.href = downloadLink;
    link.download = 'data.json'; 
    link.click();
  }
</script>

<main>
<p>{Error}</p>

<input type="file" accept=".csv" on:change={handleFileUpload} />

<br>
<button on:click={downloadValidData}>Valid Data Download</button>
<br>
<button on:click={downloadRemovedData}>Removed Data Download</button>
<br>
<button on:click={downloadJsonFile}>JSON herunterladen</button>

</main>