<template>
    <div id="k1interface-hidden" class="show">
        <div>
            <div id="device-group" class="twain-feature-group">
                <label>Device</label>
                <select v-model="selectedDevice" class="form-control" @change="onDeviceChange($event)">
                    <option v-for="option in discoveredDevices" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="documentSourceOptions.length > 0">
                <label>Document Source:</label>
                <select v-model="selectedDocumentSource" class="form-control" @change="onSourceChange($event)">
                    <option v-for="option in documentSourceOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="resolutionOptions.length > 0">
                <label>Resolution (DPI):</label>
                <select v-model="selectedResolutionOption" class="form-control">
                    <option v-for="option in resolutionOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="pixelTypeOptions.length > 0">
                <label>Color:</label>
                <select v-model="selectedPixelTypeOption" class="form-control">
                    <option v-for="option in pixelTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="pageSizeOptions.length > 0">
                <label>Page Size:</label>
                <select v-model="selectedPageSizeOption" class="form-control">
                    <option v-for="option in pageSizeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="duplexOptions.length > 0">
                <label>Duplex Option:</label>
                <select v-model="selectedDuplexOption" class="form-control">
                    <option v-for="option in duplexOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <label>Output File Name</label>
            <input v-model="outputFilename" id="sel-output-name" class="form-control" type="text" placeholder="Please enter a file name" />

            <label>Output File Type</label>
            <select v-model="selectedFileTypeOption" class="form-control">
                <option v-for="option in fileTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                    {{ option.display }}
                </option>
            </select>

            <label>OCR Type</label>
            <select v-model="selectedOcrOption" class="form-control">
                <option v-for="option in ocrOptions" v-bind:key="option.value" v-bind:value="option.value">
                    {{ option.display }}
                </option>
            </select>

            <br />

            <div class="input-group">
                <div class="input-group-btn">
                    <button id="btn-acquire" type="button" class="btn btn-primary" aria-label="Bold" v-on:click="acquire">
                        <span>Scan</span>
                    </button>
                </div>
                <input class="form-control filename" aria-label="Text input with multiple buttons" />
            </div>
        </div>

        <br />
        <ScanCompleted v-if="acquireResponse != null" :msg="JSON.stringify(acquireResponse, null, 4)"></ScanCompleted>
        <ScanError v-if="acquireError != null" :msg="acquireError"></ScanError>
    </div>

</template>

<script>
    import $ from 'jquery'
    import '../lib/k1scanservice/css/k1ss.min.css';
    import ScanCompleted from './ScanCompleted'
    import ScanError from './ScanError'

    let K1WebTwainModule = require('../lib/k1scanservice/js/k1ss_framework.js');
    let K1WebTwain = K1WebTwainModule.K1WebTwain;

    export default {
        name: 'ScannerInterfaceHidden',
        components: {
            ScanCompleted,
            ScanError
        },
        data: function () {
            return {
                discoveredDevices: [],
                selectedDevice: -1,
                selectedDeviceObj: null,
                documentSourceOptions: [],
                selectedDocumentSource: -1,
                duplexOptions: [],
                selectedDuplexOption: -1,
                pageSizeOptions: [],
                selectedPageSizeOption: -1,
                pixelTypeOptions: [],
                selectedPixelTypeOption: -1,
                resolutionOptions: [],
                selectedResolutionOption: -1,
                ocrOptions: [],
                selectedOcrOption: -1,
                fileTypeOptions: [],
                selectedFileTypeOption: -1,
                outputFilename: "",
                acquireResponse: null,
                acquireError: null,
            }
        },
        methods: {
            onDeviceChange: function (e) {
                let device = K1WebTwain.Device(e.target.value);

                let documentSourceOptions = [];

                if (device !== null) {
                    documentSourceOptions = Object.keys(device.documentSourceIds).map((key) => {
                        return { value: key, display: device.documentSourceIds[key].name };
                    });
                }

                this.selectedDeviceObj = device;
                this.selectedDocumentSource = -1;
                this.duplexOptions = [];
                this.pageSizeOptions = [];
                this.pixelTypeOptions = [];
                this.resolutionOptions = [];
                this.documentSourceOptions = [{ value: -1, display: "Please Select" }].concat(documentSourceOptions);
            },
            onSourceChange: function (e) {
                let selectedDocumentSource = this.selectedDeviceObj.documentSourceIds[e.target.value];

                let duplexOptions = [],
                    pageSizeOptions = [],
                    pixelTypeOptions = [],
                    resolutionOptions = [];

                if (selectedDocumentSource !== null || selectedDocumentSource !== undefined) {
                    duplexOptions = Object.keys(selectedDocumentSource.duplexIds).map((key) => {
                        return { value: key, display: selectedDocumentSource.duplexIds[key] };
                    });

                    if (duplexOptions.length > 0) {
                        duplexOptions = [{ value: -1, display: "Please Select" }].concat(duplexOptions)
                    }

                    pageSizeOptions = Object.keys(selectedDocumentSource.pageSizeIds).map((key) => {
                        return { value: key, display: selectedDocumentSource.pageSizeIds[key] };
                    });

                    if (pageSizeOptions.length > 0) {
                        pageSizeOptions = [{ value: -1, display: "Please Select" }].concat(pageSizeOptions)
                    }

                    pixelTypeOptions = Object.keys(selectedDocumentSource.pixelTypeIds).map((key) => {
                        return { value: key, display: selectedDocumentSource.pixelTypeIds[key] };
                    });

                    if (pixelTypeOptions.length > 0) {
                        pixelTypeOptions = [{ value: -1, display: "Please Select" }].concat(pixelTypeOptions)
                    }

                    resolutionOptions = Object.keys(selectedDocumentSource.resolutionIds).map((key) => {
                        return { value: key, display: selectedDocumentSource.resolutionIds[key] };
                    });

                    if (resolutionOptions.length > 0) {
                        resolutionOptions = [{ value: -1, display: "Please Select" }].concat(resolutionOptions)
                    }
                }
                else {
                    selectedDocumentSource = null;
                }

                this.selectedDuplexOption = -1;
                this.selectedPageSizeOption = -1;
                this.selectedPixelTypeOption = -1;
                this.selectedResolutionOption = -1;
                this.duplexOptions = duplexOptions;
                this.pageSizeOptions = pageSizeOptions;
                this.pixelTypeOptions = pixelTypeOptions;
                this.resolutionOptions = resolutionOptions;
            },
            acquire: function () {
                this.acquireResponse = null;
                this.acquireError = null;

                let acquireRequest = {
                    deviceId: this.selectedDevice,
                    resolutionId: this.selectedResolutionOption,
                    pixelTypeId: this.selectedPixelTypeOption,
                    pageSizeId: this.selectedPageSizeOption,
                    documentSourceId: this.selectedDocumentSource,
                    duplexId: this.selectedDuplexOption,
                    filetype: this.selectedFileTypeOption,
                    ocrType: this.selectedOcrOption,
                    filename: this.outputFilename,
                };

                window.console.log(acquireRequest);

                K1WebTwain.Acquire(acquireRequest)
                    .then(response => {
                        window.console.log(response);
                        this.acquireResponse = response;
                    })
                    .catch(err => {
                        window.console.error(err);
                        let myError = null;

                        if (err.responseText) {
                            myError = err.responseText;
                        }

                        if (err.responseJSON) {
                            try {
                                myError = JSON.stringify(err.responseJSON, null, 4);
                            }
                            catch (e) {
                                window.console.warn(e);
                            }
                        }

                        this.acquireError = myError;
                    });
            }
        },
        props: {
        },
        mounted: function () {
            window.console.log("ScannerInterfaceDesktop");

            //let K1WebTwain = K1WebTwainModule.K1WebTwain;
            window.console.log(K1WebTwain);
            var configuration = {
                onComplete: function () { },
                viewButton: $(".k1ViewBtn"),
                fileUploadURL: document.location.origin + '/home/uploadfile',
                clientID: "" + Date.now(),
                setupFile: document.location.origin + '/Home/DownloadSetup',
                interfacePath: document.location.origin + "/interface.html",
                scannerInterface: K1WebTwain.Options.ScannerInterface.Hidden,
                scanButton: $("#scanbtn"),
            };

            K1WebTwain.Configure(configuration).then(x => {
                window.console.log(x);
                K1WebTwain.GetDevices().then(devices => {
                    window.console.log(devices);
                    let mappedDevices = devices.map(device => {
                        return { value: device.id, display: device.name };
                    });

                    if (mappedDevices.length > 0) {
                        mappedDevices = [{ value: -1, display: "Please Select" }].concat(mappedDevices);
                    }

                    let mappedOcrTypes = Object.keys(K1WebTwain.Options.OcrType).map((key) => {
                        return { value: K1WebTwain.Options.OcrType[key], display: key };
                    });

                    if (mappedOcrTypes.length > 0) {
                        mappedOcrTypes = [{ value: -1, display: "Please Select" }].concat(mappedOcrTypes);
                    }

                    let mappedFileTypeOptions = Object.keys(K1WebTwain.Options.OutputFiletype).map((key) => {
                        return { value: K1WebTwain.Options.OutputFiletype[key], display: key };
                    });

                    if (mappedFileTypeOptions.length > 0) {
                        mappedFileTypeOptions = [{ value: -1, display: "Please Select" }].concat(mappedFileTypeOptions);
                    }

                    this.discoveredDevices = mappedDevices;
                    this.ocrOptions = mappedOcrTypes;
                    this.fileTypeOptions = mappedFileTypeOptions;
                }).catch(err => {
                    window.console.error(err);
                });
            }).catch(x => {
                window.console.log(x);
            });
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    h3 {
        margin: 40px 0 0;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        display: inline-block;
        margin: 0 10px;
    }

    a {
        color: #42b983;
    }
</style>
