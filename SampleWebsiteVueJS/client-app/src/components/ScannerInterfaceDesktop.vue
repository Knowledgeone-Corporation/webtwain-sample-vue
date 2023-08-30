<template>
    <div id="k1interface-hidden" class="show" v-if="isDisplayUI">
        <div>
            <div id="device-group" class="twain-feature-group">
                <label class="scanning-label">Device</label>
                <select v-model="selectedDevice" class="form-control">
                    <option v-for="option in discoveredDevices" v-bind:key="option.value" v-bind:value="option.value">
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
            <select v-model="selectedOcrType" class="form-control">
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
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss_obfuscated.js'
    import { convertRawOptions, generateScanFileName, renderOptions } from '../utils/scanningUtils.js'
    
    export default {
        name: 'ScannerInterfaceDesktop',
        data: function () {
            return {
                outputFilename: '',
                selectedDevice: 0,
                selectedOcrType: K1WebTwain.Options.OcrType.None,
                selectedFileTypeOption: K1WebTwain.Options.OutputFiletype.PDF,
                discoveredDevices: [],
                ocrOptions: [],
                fileTypeOptions: [],
                isDisplayUI: false
            }
        },
        methods: {
            acquire: function () {
                this.isDisplayUI = false;
                let acquireRequest = {
                    deviceId: this.selectedDevice,
                    filetype: this.selectedFileTypeOption,
                    ocrType: this.selectedOcrType,
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
