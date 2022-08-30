<template>
    <div id="k1interface-hidden" class="show" v-if="isDisplayUI">
        <div>
            <div id="device-group" class="twain-feature-group">
                <label class="scanning-label">Device</label>
                <select v-model="selectedDevice" class="form-control" @change="onDeviceChange($event.target.value)">
                    <option v-for="option in discoveredDevices" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="documentSourceOptions.length > 0">
                <label class="scanning-label mt-2">Document Source:</label>
                <select v-model="selectedDocumentSource" class="form-control" @change="onDocumentSourceChange($event.target.value)">
                    <option v-for="option in documentSourceOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="resolutionOptions.length > 0">
                <label class="scanning-label mt-2">Resolution (DPI):</label>
                <select v-model="selectedResolutionOption" class="form-control">
                    <option v-for="option in resolutionOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="pixelTypeOptions.length > 0">
                <label class="scanning-label mt-2">Color:</label>
                <select v-model="selectedPixelTypeOption" class="form-control">
                    <option v-for="option in pixelTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="pageSizeOptions.length > 0">
                <label class="scanning-label mt-2">Page Size:</label>
                <select v-model="selectedPageSizeOption" class="form-control">
                    <option v-for="option in pageSizeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <div id="size-group" class="twain-feature-group" v-if="duplexOptions.length > 0">
                <label class="scanning-label mt-2">Duplex Option:</label>
                <select v-model="selectedDuplexOption" class="form-control">
                    <option v-for="option in duplexOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <label class="scanning-label mt-2">Output File Name</label>
            <input v-model="outputFilename" id="sel-output-name" class="form-control" type="text" placeholder="Please enter a file name" />

            <label class="scanning-label mt-2">Output File Type</label>
            <select v-model="selectedFileTypeOption" class="form-control">
                <option v-for="option in fileTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                    {{ option.display }}
                </option>
            </select>

            <label class="scanning-label mt-2">OCR Type</label>
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
            </div>
        </div>
    </div>

</template>

<script>
    import $ from 'jquery'
    import { isEmpty } from 'lodash'
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss_obfuscated.js'
    import { convertRawOptions, defaultOptionsValue, generateScanFileName, renderOptions } from '../utils/scanningUtils.js'

    export default {
        name: 'ScannerInterfaceHidden',
        data: function () {
            return {
                discoveredDevices: [],
                selectedDevice: 0,
                selectedDeviceObj: {},
                documentSourceOptions: [],
                selectedDocumentSource: 0,
                duplexOptions: [],
                selectedDuplexOption: 0,
                pageSizeOptions: [],
                selectedPageSizeOption: 0,
                pixelTypeOptions: [],
                selectedPixelTypeOption: 0,
                resolutionOptions: [],
                selectedResolutionOption: 0,
                ocrOptions: [],
                selectedOcrOption: K1WebTwain.Options.OcrType.None,
                fileTypeOptions: [],
                selectedFileTypeOption: K1WebTwain.Options.OutputFiletype.PDF,
                outputFilename: '',
                isDisplayUI: false
            }
        },
        methods: {
            onDeviceChange: function (deviceId) {
                K1WebTwain.Device(deviceId).then(deviceInfo => {
                    if(!isEmpty(deviceInfo)) {
                        let documentSourceOptions = Object.keys(deviceInfo.documentSourceIds).map((key) => {
                            return { value: key, display: deviceInfo.documentSourceIds[key].name };
                        });

                        this.selectedDevice = deviceId;
                        this.selectedDeviceObj = deviceInfo;
                        this.selectedDocumentSource = defaultOptionsValue(documentSourceOptions);
                        this.duplexOptions = [];
                        this.pageSizeOptions = [];
                        this.pixelTypeOptions = [];
                        this.resolutionOptions = [];
                        this.documentSourceOptions = renderOptions(documentSourceOptions);

                        this.onDocumentSourceChange(defaultOptionsValue(documentSourceOptions));
                    }
                    }).catch(err => {
                        window.console.log(err);
                        this.selectedDevice = deviceId;
                        this.selectedDeviceObj = {};
                        this.selectedDocumentSource = 0;
                        this.duplexOptions = [];
                        this.pageSizeOptions = [];
                        this.pixelTypeOptions = [];
                        this.resolutionOptions = [];
                        this.documentSourceOptions = [];
                    })
                
            },
            onDocumentSourceChange: function (documentSourceId) {
                let selectedDocumentSource = this.selectedDeviceObj.documentSourceIds[documentSourceId];

                let duplexOptions = [],
                    pageSizeOptions = [],
                    pixelTypeOptions = [],
                    resolutionOptions = [];

                if (selectedDocumentSource) {
                    duplexOptions = convertRawOptions(selectedDocumentSource.duplexIds);
                    pageSizeOptions = convertRawOptions(selectedDocumentSource.pageSizeIds);
                    pixelTypeOptions = convertRawOptions(selectedDocumentSource.pixelTypeIds);
                    resolutionOptions = convertRawOptions(selectedDocumentSource.resolutionIds);
                } else {
                    selectedDocumentSource = null;
                }

                this.selectedDuplexOption = defaultOptionsValue(duplexOptions);
                this.selectedPageSizeOption = defaultOptionsValue(pageSizeOptions);
                this.selectedPixelTypeOption = defaultOptionsValue(pixelTypeOptions);
                this.selectedResolutionOption = defaultOptionsValue(resolutionOptions);
                this.selectedDocumentSource = documentSourceId;
                this.duplexOptions = renderOptions(duplexOptions);
                this.pageSizeOptions = renderOptions(pageSizeOptions);
                this.pixelTypeOptions = renderOptions(pixelTypeOptions);
                this.resolutionOptions = renderOptions(resolutionOptions);
            },
            acquire: function () {
                this.isDisplayUI = false;
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

                K1WebTwain.Acquire(acquireRequest)
                    .then(response => {
                        this.$parent.completeAcquire({
                            acquireResponse: JSON.stringify(response.uploadResponse, null, 4),
                            acquireError: '',
                        });
                    })
                    .catch(err => {
                        if(err) {
                            if (err.responseText) {
                                this.$parent.completeAcquire({
                                    acquireResponse: '',
                                    acquireError: err.responseText,
                                });
                            }

                            if (err.responseJSON) {
                                try {
                                    this.$parent.completeAcquire({
                                        acquireResponse: '',
                                        acquireError: JSON.stringify(err.responseJSON, null, 4),
                                    });
                                } catch (e) {
                                    window.console.warn(e);
                                }
                            }

                            if (err.statusText && err.statusText === 'timeout') {
                                this.$parent.completeAcquire({
                                    acquireResponse: '',
                                    acquireError: 'Timeout error while processing/uploading scanned documents.',
                                });
                            }
                        }
                    });
            },
            renderSelection: function () {
                K1WebTwain.GetDevices().then(devices => {
                    let mappedDevices = devices.map(device => ({ value: device.id, display: device.name }));
                    let mappedOcrTypes = convertRawOptions(K1WebTwain.Options.OcrType, true);
                    let mappedFileTypeOptions = convertRawOptions(K1WebTwain.Options.OutputFiletype, true);

                    this.ocrOptions = renderOptions(mappedOcrTypes);
                    this.fileTypeOptions = renderOptions(mappedFileTypeOptions);
                    this.discoveredDevices = renderOptions(mappedDevices);
                    this.outputFilename = generateScanFileName();

                    this.onDeviceChange(defaultOptionsValue(mappedDevices));
                }).catch(err => {
                    window.console.error(err);
                });
            }
        },
        props: {
        },
        mounted: function () {
            let self = this;
            let configuration = {
                onComplete: function () { }, //function called when scan complete
                viewButton: null, //This is optional. Specify a element that when clicked will view scanned document
                fileUploadURL: document.location.origin + '/Home/UploadFile', //This is the service that the scanned document will be uploaded to when complete
                clientID: "" + Date.now(), //This is a way to identify the user who is scanning.  It should be unique per user.  Session ID could be used if no user logged in
                setupFile: document.location.origin + '/Home/DownloadSetup', //location of the installation file if service doesn't yet exist
                interfacePath: document.location.origin + "/interface.html", // This is optional if your application lives under a subdomain.
                scannerInterface: K1WebTwain.Options.ScannerInterface.Desktop,
                scanButton: $("#scanbtn"), // the scan button
            };

            K1WebTwain.Configure(configuration).then(() => {
                this.isDisplayUI = false;

                K1WebTwain.ResetService().then(function () {
                    setTimeout(() => {
                        self.renderSelection();
                        self.isDisplayUI = true;
                    },4000)
                });
            }).catch(err => {
                window.console.log(err);
                K1WebTwain.ResetService();
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
