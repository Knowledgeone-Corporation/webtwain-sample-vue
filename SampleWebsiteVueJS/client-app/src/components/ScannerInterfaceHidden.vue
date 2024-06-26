<template>
    <div v-if="isDisplayUI">
        <div id="k1interface-visible" v-if="!isDisplayScanningSection" class="show">
            <div><label class="scanning-label">Initialize Scan Process:</label></div>
            <div class="input-group">
                <div class="input-group-btn">
                    <button id="scanbtn" type="button" class="btn btn-primary" aria-label="Bold"
                        v-on:click="renderSelection">
                        <span>Initialize</span>
                    </button>
                </div>
            </div>
        </div>
        <div id="k1interface-hidden" v-if="isDisplayScanningSection" class="show">
            <div>
                <div id="device-group" class="twain-feature-group">
                    <label class="scanning-label">Device</label>
                    <select v-model="selectedDeviceId" class="form-control"
                        @change="handleDeviceChange($event.target.value)">
                        <option v-for="option in discoveredDevices" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="documentSourceOptions.length > 0">
                    <label class="scanning-label mt-2">Document Source:</label>
                    <select v-model="selectedDocumentSource" class="form-control"
                        @change="onDocumentSourceChange($event.target.value)">
                        <option v-for="option in documentSourceOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="resolutionOptions.length > 0">
                    <label class="scanning-label mt-2">Resolution (DPI):</label>
                    <select v-model="selectedResolutionOption" class="form-control">
                        <option v-for="option in resolutionOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="pixelTypeOptions.length > 0">
                    <label class="scanning-label mt-2">Color:</label>
                    <select v-model="selectedPixelTypeOption" class="form-control">
                        <option v-for="option in pixelTypeOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
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
                <br />
                <div class="input-group">
                    <div class="input-group-btn">
                        <button id="btn-acquire" type="button" class="btn btn-primary" aria-label="Bold"
                            v-on:click="acquire" :disabled="isDisableScanButton">
                            <span>Scan</span>
                        </button>
                    </div>
                </div>
            </div>
            <div :class="{ 'section-disabled': isDisableFinalizeSection }">
                <label class="scanning-label mt-2">Output File Name</label>
                <input v-model="outputFilename" id="sel-output-name" class="form-control" type="text"
                    placeholder="Please enter a file name" />
                <label class="scanning-label mt-2">Output File Type</label>
                <span class="filetype-restriction" v-if="isDisplayFileRestriction">
                    File types restricted for multiple page scans
                </span>
                <select v-model="selectedFileTypeOption" class="form-control"
                    @change="handleFileTypeChange($event.target.value)">
                    <option v-for="option in fileTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
                <div v-if="isDisplayOCR">
                    <label class="scanning-label mt-2">OCR Type</label>
                    <select v-model="selectedOcrOption" class="form-control"
                        @change="handlOcrTypeChange($event.target.value)">
                        <option v-for="option in ocrOptions" v-bind:key="option.value" v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div class="input-group mb-3 mt-3">
                    <div class="input-group-btn">
                        <button id="btn-attach" type="button" class="btn btn-primary" aria-label="Bold"
                            v-on:click="handleAttachDocument()">
                            <span>ATTACH DOCUMENT</span>
                        </button>
                        <button id="btn-save-locally" type="button" class="btn btn-primary ml-2" aria-label="Bold"
                            v-on:click="handleSaveDocument()">
                            <span>SAVE LOCALLY</span>
                        </button>
                        <button id="btn-cancel-finalization" type="button" class="btn btn-primary ml-2"
                            aria-label="Bold" v-on:click="handleCancelFinalization()">
                            <span>CANCEL</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import $ from 'jquery'
import { isEmpty } from 'lodash'
import { K1WebTwain } from '../lib/k1scanservice/js/k1ss.js'
import {
    convertRawOptions, defaultOptionsValue, saveDefaultScanSettings,
    getDefaultScanSettings, generateScanFileName, renderOptions
} from '../utils/scanningUtils.js'

export default {
    name: 'ScannerInterfaceHidden',
    data: function () {
        return {
            discoveredDevices: [],
            selectedDeviceId: -1,
            selectedDeviceInfo: {},
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
            isDisplayUI: false,
            isDisplayScanningSection: false,
            isDisableScanButton: true,
            isDisableFinalizeSection: true,
            isDisplayFileRestriction: false,
            isDisplayOCR: false
        }
    },
    methods: {
        handleDeviceChange: function (deviceId) {
            K1WebTwain.Device(deviceId).then(deviceInfo => {
                if (!isEmpty(deviceInfo)) {
                    let documentSourceOptions = Object.keys(deviceInfo.documentSourceIds).map((key) => {
                        return { value: key, display: deviceInfo.documentSourceIds[key].name };
                    });

                    this.selectedDeviceId = deviceId;
                    this.selectedDeviceInfo = deviceInfo;
                    this.selectedDocumentSource = defaultOptionsValue(documentSourceOptions);
                    this.duplexOptions = [];
                    this.pageSizeOptions = [];
                    this.pixelTypeOptions = [];
                    this.resolutionOptions = [];
                    this.documentSourceOptions = renderOptions(documentSourceOptions);
                    this.isDisableScanButton = false;

                    this.onDocumentSourceChange(defaultOptionsValue(documentSourceOptions));
                } else {
                    this.selectedDeviceId = -1;
                    this.selectedDeviceInfo = {};
                    this.selectedDocumentSource = 0;
                    this.duplexOptions = [];
                    this.pageSizeOptions = [];
                    this.pixelTypeOptions = [];
                    this.resolutionOptions = [];
                    this.documentSourceOptions = [];
                    this.isDisableScanButton = true;
                }

                let defaultSettings = getDefaultScanSettings();
                saveDefaultScanSettings(
                    defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                    defaultSettings?.OCRType ?? this.selectedOcrOption,
                    deviceId
                );
            }).catch(err => {
                window.console.log(err);
                this.selectedDeviceId = deviceId;
                this.selectedDeviceInfo = {};
                this.selectedDocumentSource = 0;
                this.duplexOptions = [];
                this.pageSizeOptions = [];
                this.pixelTypeOptions = [];
                this.resolutionOptions = [];
                this.documentSourceOptions = [];
                this.isDisableScanButton = true;
            })
        },
        onDocumentSourceChange: function (documentSourceId) {
            let selectedDocumentSource = this.selectedDeviceInfo.documentSourceIds[documentSourceId];

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
            let acquireRequest = {
                deviceId: this.selectedDeviceId,
                resolutionId: this.selectedResolutionOption,
                pixelTypeId: this.selectedPixelTypeOption,
                pageSizeId: this.selectedPageSizeOption,
                documentSourceId: this.selectedDocumentSource,
                duplexId: this.selectedDuplexOption,
            };

            K1WebTwain.StartScan(acquireRequest)
                .then(response => {
                    if (response.pageCount > 1) {
                        this.isDisplayFileRestriction = true;
                        let fileType = this.selectedFileTypeOption;
                        if (
                            fileType === "JPG" ||
                            fileType === "GIF" ||
                            fileType === "PNG" ||
                            fileType === "BMP"
                        ) {
                            this.selectedFileTypeOption = K1WebTwain.Options.OutputFiletype.TIFF;
                        }
                        this.fileTypeOptions = this.fileTypeOptions.filter(
                            (fileType) =>
                                fileType.value === "PDF" ||
                                fileType.value === "PDF/A" ||
                                fileType.value === "TIF"
                        );
                    } else {
                        this.isDisplayFileRestriction = false;
                        let mappedFileTypeOptions = convertRawOptions(
                            K1WebTwain.Options.OutputFiletype,
                            true
                        );
                        this.fileTypeOptions = renderOptions(mappedFileTypeOptions);
                    }

                    this.isDisableFinalizeSection = false;
                    this.isDisableScanButton = true;
                })
                .catch(err => {
                    this.handleError(err);
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
                this.isDisplayScanningSection = true;

                let scanSettings = getDefaultScanSettings();
                if (scanSettings) {
                    this.selectedFileTypeOption = scanSettings.ScanType;
                    this.selectedOcrOption = scanSettings.UseOCR
                        ? scanSettings.OCRType
                        : K1WebTwain.Options.OcrType.None;
                    this.isDisableScanButton = parseInt(scanSettings.ScanSource) === -1;
                    this.handleDeviceChange(scanSettings.ScanSource);
                } else {
                    this.handleDeviceChange(defaultOptionsValue(mappedDevices));
                }

                this.isDisplayOCR =
                    this.selectedFileTypeOption ===
                    K1WebTwain.Options.OutputFiletype.PDF ||
                    this.selectedFileTypeOption ===
                    K1WebTwain.Options.OutputFiletype["PDF/A"];
            }).catch(err => {
                window.console.error(err);
            });
        },
        handleFileTypeChange: function (outputType) {
            this.selectedFileTypeOption = outputType;
            this.isDisplayOCR =
                outputType === K1WebTwain.Options.OutputFiletype.PDF ||
                outputType === K1WebTwain.Options.OutputFiletype["PDF/A"];
            let defaultSettings = getDefaultScanSettings();
            saveDefaultScanSettings(
                outputType,
                defaultSettings?.OCRType ?? this.selectedOcrOption,
                defaultSettings?.ScanSource ?? this.selectedDeviceId
            );
        },
        handlOcrTypeChange: function (ocrType) {
            this.selectedOcrOption = ocrType
            let defaultSettings = getDefaultScanSettings();
            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                ocrType,
                defaultSettings?.ScanSource ?? this.selectedDeviceId
            );
        },
        handleCancelFinalization: function () {
            K1WebTwain.ClearAllScannedPages()
                .then(() => {
                    this.isDisableFinalizeSection = true;
                    this.isDisableScanButton = false;
                })
                .catch((err) => {
                    this.handleError(err);
                });
        },
        handleAttachDocument: function () {
            K1WebTwain.ValidatePageSize({
                ocrType: this.selectedOcrOption,
                fileType: this.selectedFileTypeOption,
                saveToType: K1WebTwain.Options.SaveToType.Upload,
                generateDocument: () => {
                    this.generateDocument(K1WebTwain.Options.SaveToType.Upload);
                },
            });
        },
        handleSaveDocument: function () {
            K1WebTwain.ValidatePageSize({
                ocrType: this.selectedOcrOption,
                fileType: this.selectedFileTypeOption,
                saveToType: K1WebTwain.Options.SaveToType.Local,
                generateDocument: () => {
                    this.generateDocument(K1WebTwain.Options.SaveToType.Local);
                },
            });
        },
        generateDocument: function (saveToType) {
            K1WebTwain.GenerateDocument({
                filetype: this.selectedFileTypeOption,
                ocrType: this.selectedOcrOption,
                saveToType: saveToType,
                filename: this.outputFilename,
            })
                .then((response) => {
                    let responseMessage = response.uploadResponse;

                    if (saveToType === K1WebTwain.Options.SaveToType.Local) {
                        responseMessage = {
                            filename: response.filename,
                            fileSize: `${response.fileLength} (${response.sizeDisplay})`,
                            fileExtension: response.extension
                        };
                    }

                    this.$parent.completeAcquire({
                        acquireResponse: JSON.stringify(responseMessage, null, 4),
                        acquireError: '',
                        saveToType: this.selectedSaveToOption
                    });
                })
                .catch((err) => {
                    this.handleError(err);
                });
        },
        handleError: function (err) {
            if (err) {
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
        },
    },
    props: {
    },
    mounted: function () {
        let self = this;
        let configuration = {
            onComplete: function () { }, //function called when scan complete
            viewButton: null, //This is optional. Specify a element that when clicked will view scanned document
            fileUploadURL: document.location.origin + '/Home/UploadFile', //This is the service that the scanned document will be uploaded to when complete
            fileUploadHeaders: [
                {
                    key: "X-Access-Token",
                    value: "Test"
                }
            ], // This is optional. Specify additional headers for the request to the upload server.
            clientID: "" + Date.now(), //This is a way to identify the user who is scanning.  It should be unique per user.  Session ID could be used if no user logged in
            setupFile: document.location.origin + '/Home/DownloadSetup', //location of the installation file if service doesn't yet exist
            licenseFile: document.location.origin + '/Home/K1Licence', //location of the license file If it unset, value will fallback to Current website url + '/Home/K1Licence'
            interfacePath: document.location.origin + "/interface.html", // This is optional if your application lives under a subdomain.
            scannerInterface: K1WebTwain.Options.ScannerInterface.Hidden,
            scanButton: $("#scanbtn"), // the scan button
        };

        K1WebTwain.Configure(configuration).then(() => {
            this.isDisplayUI = false;

            K1WebTwain.ResetService().then(function () {
                //setTimeout(() => {
                self.isDisplayUI = true;
                //},4000)
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
