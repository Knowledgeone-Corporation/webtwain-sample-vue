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
                <br />
                <div class="input-group">
                    <div class="input-group-btn">
                        <button id="btn-acquire" type="button" class="btn btn-primary" aria-label="Bold"
                            v-on:click="acquire" :disabled="isDisableScanButton">
                            <span>Scan</span>
                        </button>
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
    </div>
</template>

<script>
import $ from 'jquery'
import { K1WebTwain } from '../lib/k1scanservice/js/k1ss.js'
import {
    convertRawOptions, generateScanFileName, saveDefaultScanSettings,
    getDefaultScanSettings, getScannerDetails, renderOptions
} from '../utils/scanningUtils.js'

export default {
    name: 'ScannerInterfaceDesktop',
    data: function () {
        return {
            outputFilename: '',
            selectedDeviceId: -1,
            ocrOptions: [],
            selectedOcrOption: K1WebTwain.Options.OcrType.None,
            fileTypeOptions: [],
            selectedFileTypeOption: K1WebTwain.Options.OutputFiletype.PDF,
            discoveredDevices: [],
            isDisplayUI: false,
            isDisplayScanningSection: false,
            isDisableScanButton: true,
            isDisableFinalizeSection: true,
            isDisplayOCR: false
        }
    },
    methods: {
        handleDeviceChange: function (deviceId) {
            this.isDisableScanButton = parseInt(deviceId) === -1;

            const defaultSettings = getDefaultScanSettings();
            let scannerDetails = getScannerDetails(defaultSettings);
            scannerDetails.ScanSource = parseInt(deviceId);

            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                defaultSettings?.OCRType ?? this.selectedOcrOption,
                scannerDetails
            );
        },
        acquire: function () {
            let acquireRequest = {
                deviceId: this.selectedDeviceId,
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

                this.ocrOptions = renderOptions(mappedOcrTypes);
                this.fileTypeOptions = renderOptions(mappedFileTypeOptions);
                this.discoveredDevices = renderOptions(mappedDevices);
                this.outputFilename = generateScanFileName();
                this.isDisplayScanningSection = true;

                const scanSettings = getDefaultScanSettings();
                const deviceId = scanSettings?.ScannerDetails?.ScanSource;

                if (deviceId) {
                    this.selectedDeviceId = deviceId;
                    this.selectedFileTypeOption = scanSettings.ScanType;
                    this.selectedOcrOption = scanSettings.UseOCR
                        ? scanSettings.OCRType
                        : K1WebTwain.Options.OcrType.None;
                    this.isDisableScanButton = parseInt(deviceId) === -1;
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
            const defaultSettings = getDefaultScanSettings();

            saveDefaultScanSettings(
                outputType,
                defaultSettings?.OCRType ?? this.selectedOcrOption
            );
        },
        handlOcrTypeChange: function (ocrType) {
            this.selectedOcrOption = ocrType
            const defaultSettings = getDefaultScanSettings();

            saveDefaultScanSettings(
                defaultSettings?.ScanType ?? this.selectedFileTypeOption,
                ocrType
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
            scannerInterface: K1WebTwain.Options.ScannerInterface.Desktop,
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
