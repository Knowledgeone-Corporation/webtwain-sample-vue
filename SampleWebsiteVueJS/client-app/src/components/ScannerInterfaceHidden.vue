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
                        @change="handleDocumentSourceChange($event.target.value)">
                        <option v-for="option in documentSourceOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="resolutionOptions.length > 0">
                    <label class="scanning-label mt-2">Resolution (DPI):</label>
                    <select v-model="selectedResolutionOption" class="form-control"
                        @change="handleResolutionChange($event.target.value)">
                        <option v-for="option in resolutionOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="pixelTypeOptions.length > 0">
                    <label class="scanning-label mt-2">Color:</label>
                    <select v-model="selectedPixelTypeOption" class="form-control"
                        @change="handlePixelTypeChange($event.target.value)">
                        <option v-for="option in pixelTypeOptions" v-bind:key="option.value"
                            v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="pageSizeOptions.length > 0">
                    <label class="scanning-label mt-2">Page Size:</label>
                    <select v-model="selectedPageSizeOption" class="form-control"
                        @change="handlePageSizeChange($event.target.value)">
                        <option v-for="option in pageSizeOptions" v-bind:key="option.value" v-bind:value="option.value">
                            {{ option.display }}
                        </option>
                    </select>
                </div>
                <div id="size-group" class="twain-feature-group" v-if="duplexOptions.length > 0">
                    <label class="scanning-label mt-2">Duplex Option:</label>
                    <select v-model="selectedDuplexOption" class="form-control"
                        @change="handleDuplexChange($event.target.value)">
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
                <select v-model="selectedFileTypeOption" class="form-control"
                    @change="handleFileTypeChange($event.target.value)">
                    <option v-for="option in fileTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>

                <label class="scanning-label mt-2">Output File Compression</label>
                <select v-model="selectedCompressionOption" class="form-control"
                    @change="handleCompressionChange($event.target.value)">
                    <option v-for="option in compressionOptions" v-bind:key="option.value" v-bind:value="option.value">
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
    getDefaultScanSettings, getScannerDetails, generateScanFileName, renderOptions
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
            compressionOptions: [],
            selectedCompressionOption: "",
            outputFilename: '',
            isDisplayUI: false,
            isDisplayScanningSection: false,
            isDisableScanButton: true,
            isDisableFinalizeSection: true,
            isDisplayOCR: false
        }
    },
    methods: {
        handleDeviceChange: function (deviceId) {
            K1WebTwain.Device(deviceId).then(deviceInfo => {
                let defaultSettings = getDefaultScanSettings();

                if (!isEmpty(deviceInfo)) {
                    let documentSourceOptions = Object.keys(deviceInfo.documentSourceIds).map((key) => {
                        return { value: key, display: deviceInfo.documentSourceIds[key].name };
                    });

                    const defaultDocumentSource = defaultSettings?.ScannerDetails?.DocumentSource ?? defaultOptionsValue(documentSourceOptions);

                    this.selectedDeviceId = deviceId;
                    this.selectedDeviceInfo = deviceInfo;
                    this.selectedDocumentSource = defaultDocumentSource;
                    this.duplexOptions = [];
                    this.pageSizeOptions = [];
                    this.pixelTypeOptions = [];
                    this.resolutionOptions = [];
                    this.documentSourceOptions = renderOptions(documentSourceOptions);
                    this.isDisableScanButton = false;

                    this.handleDocumentSourceChange(defaultDocumentSource);
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

                let scannerDetails = getScannerDetails(defaultSettings);
                scannerDetails.ScanSource = deviceId;
                this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
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
        saveDefaultScannerDetails: function (defaultSettings, scannerDetails) {
            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                defaultSettings?.OCRType ?? this.selectedOcrOption,
                scannerDetails
            );
        },
        handleDocumentSourceChange: function (documentSourceId) {
            let defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);

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

            this.selectedDuplexOption = scannerDetails?.Duplex ?? defaultOptionsValue(duplexOptions);
            this.selectedPageSizeOption = scannerDetails?.PageSize ?? defaultOptionsValue(pageSizeOptions);
            this.selectedPixelTypeOption = scannerDetails?.Color ?? defaultOptionsValue(pixelTypeOptions);
            this.selectedResolutionOption = scannerDetails?.Resolution ?? defaultOptionsValue(resolutionOptions);
            this.selectedDocumentSource = documentSourceId;
            this.duplexOptions = renderOptions(duplexOptions);
            this.pageSizeOptions = renderOptions(pageSizeOptions);
            this.pixelTypeOptions = renderOptions(pixelTypeOptions);
            this.resolutionOptions = renderOptions(resolutionOptions);

            scannerDetails.DocumentSource = documentSourceId;
            this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
        },
        handleResolutionChange: function (selectedResolution) {
            this.selectedResolutionOption = selectedResolution;
            let defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);
            scannerDetails.Resolution = selectedResolution;
            this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
        },
        handlePixelTypeChange: function (selectedPixelType) {
            this.selectedPixelTypeOption = selectedPixelType;
            let defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);
            scannerDetails.Color = selectedPixelType;
            this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
        },
        handlePageSizeChange: function (selectedPageSize) {
            this.selectedPageSizeOption = selectedPageSize;
            let defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);
            scannerDetails.PageSize = selectedPageSize;
            this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
        },
        handleDuplexChange: function (selectedDuplex) {
            this.selectedDuplexOption = selectedDuplex;
            let defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);
            scannerDetails.Duplex = selectedDuplex;
            this.saveDefaultScannerDetails(defaultSettings, scannerDetails);
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
                .then(() => {
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
                let mappedCompressionOptions = convertRawOptions(K1WebTwain.Options.FileCompressionType, true);

                this.ocrOptions = renderOptions(mappedOcrTypes);
                this.fileTypeOptions = renderOptions(mappedFileTypeOptions);
                this.compressionOptions = renderOptions(mappedCompressionOptions);
                this.selectedCompressionOption = mappedCompressionOptions.length > 0 ? mappedCompressionOptions[0].value : "";
                this.discoveredDevices = renderOptions(mappedDevices);
                this.outputFilename = generateScanFileName();
                this.isDisplayScanningSection = true;

                const scanSettings = getDefaultScanSettings();

                if (scanSettings) {
                    const deviceId = scanSettings?.ScannerDetails?.ScanSource ? parseInt(scanSettings?.ScannerDetails?.ScanSource) : -1;
                    this.selectedFileTypeOption = scanSettings.ScanType;
                    this.selectedOcrOption = scanSettings.UseOCR ? scanSettings.OCRType : K1WebTwain.Options.OcrType.None;
                    this.selectedCompressionOption = scanSettings.FileCompressionType ?? (mappedCompressionOptions.length > 0 ? mappedCompressionOptions[0].value : "");
                    this.isDisableScanButton = deviceId === -1;
                    this.handleDeviceChange(deviceId);
                } else {
                    this.handleDeviceChange(defaultOptionsValue(mappedDevices));
                }

                this.isDisplayOCR =
                    this.selectedFileTypeOption === K1WebTwain.Options.OutputFiletype.PDF ||
                    this.selectedFileTypeOption === K1WebTwain.Options.OutputFiletype["PDF/A"];
            }).catch(err => {
                window.console.error(err);
            });
        },
        handleFileTypeChange: function (outputType) {
            this.selectedFileTypeOption = outputType;
            this.isDisplayOCR = outputType === K1WebTwain.Options.OutputFiletype.PDF || outputType === K1WebTwain.Options.OutputFiletype["PDF/A"];
            const defaultSettings = getDefaultScanSettings();
            saveDefaultScanSettings(
                outputType,
                defaultSettings?.OCRType ?? this.selectedOcrOption,
                null,
                this.selectedCompressionOption
            );
        },
        handlOcrTypeChange: function (ocrType) {
            this.selectedOcrOption = ocrType
            const defaultSettings = getDefaultScanSettings();
            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                ocrType,
                null,
                this.selectedCompressionOption
            );
        },
        handleCompressionChange: function (compressionType) {
            this.selectedCompressionOption = compressionType;
            const defaultSettings = getDefaultScanSettings();
            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                defaultSettings?.OCRType ?? this.selectedOcrOption,
                null,
                compressionType
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
                fileCompressionType: this.selectedCompressionOption,
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
                fileCompressionType: this.selectedCompressionOption,
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
                fileCompressionType: this.selectedCompressionOption,
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
