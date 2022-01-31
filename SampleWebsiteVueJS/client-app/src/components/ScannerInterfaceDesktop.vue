<template>
    <div id="k1interface-hidden" class="show">
        <div>
            <div id="device-group" class="twain-feature-group">
                <label>Device</label>
                <select v-model="selectedScanner" class="form-control">
                    <option v-for="option in mappedDevices" v-bind:key="option.value" v-bind:value="option.value">
                        {{ option.display }}
                    </option>
                </select>
            </div>

            <label>Output File Name</label>
            <input v-model="outputFilename" id="sel-output-name" class="form-control" type="text" placeholder="Please enter a file name" />

            <label>Output File Type</label>
            <select v-model="selectedFileTypeOption" class="form-control">
                <option v-for="option in mappedFileTypeOptions" v-bind:key="option.value" v-bind:value="option.value">
                    {{ option.display }}
                </option>
            </select>

            <label>OCR Type</label>
            <select v-model="selectedOcrType" class="form-control">
                <option v-for="option in mappedOcrTypes" v-bind:key="option.value" v-bind:value="option.value">
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
    let K1WebTwainModule = require('../lib/k1scanservice/js/k1ss_framework.js');
    let K1WebTwain = K1WebTwainModule.K1WebTwain;
    import ScanCompleted from './ScanCompleted'
    import ScanError from './ScanError'

    export default {
        name: 'ScannerInterfaceDesktop',
        components: {
            ScanCompleted,
            ScanError
        },
        data: function () {
            return {
                outputFilename: "",
                acquireResponse: null,
                acquireError: null,
                selectedScanner: -1,
                selectedOcrType: -1,
                selectedFileTypeOption: -1,
                mappedDevices: [],
                mappedOcrTypes: [],
                mappedFileTypeOptions: [],
            }
        },
        methods: {
            acquire: function () {
                this.acquireResponse = null;
                this.acquireError = null;

                let acquireRequest = {
                    deviceId: this.selectedScanner,
                    filetype: this.selectedFileTypeOption,
                    ocrType: this.selectedOcrType,
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
                        window.console.error(this.acquireError);
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
                fileUploadURL: 'http://localhost:50598/home/uploadfile',
                clientID: "" + Date.now(),
                setupFile: document.location.origin + '/Home/DownloadSetup',
                interfacePath: document.location.origin + "/interface.html",
                scannerInterface: K1WebTwain.Options.ScannerInterface.Desktop,
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

                    this.mappedDevices = mappedDevices;
                    this.mappedOcrTypes = mappedOcrTypes;
                    this.mappedFileTypeOptions = mappedFileTypeOptions;
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
