<template>
    <div id="k1interface-visible" class="show">
        <div><label>Name:</label></div>
        <input asp-for="Name" class="form-control" />

        <div><label>Title:</label></div>
        <input asp-for="Title" class="form-control" />

        <div><label>Scan Document:</label></div>
        <div class="input-group">
            <div class="input-group-btn">
                <button id="scanbtn" type="button" class="btn btn-primary" aria-label="Bold">
                    <span>Scan</span>
                </button>
            </div>
            <input class="form-control filename" aria-label="Text input with multiple buttons" />
        </div>

        <br />

        <ScanCompleted v-if="acquireResponse != null" :msg="JSON.stringify(acquireResponse, null, 4)"></ScanCompleted>
    </div>
</template>

<script>
    import $ from 'jquery'
    import '../lib/k1scanservice/css/k1ss.min.css';
    let K1WebTwainModule = require('../lib/k1scanservice/js/k1ss_framework.js');
    let K1WebTwain = K1WebTwainModule.K1WebTwain;
    import ScanCompleted from './ScanCompleted';

    export default {
        name: 'ScannerInterfaceWeb',
        components: {
            ScanCompleted
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
        methods: {},
        props: {
        },
        mounted: function () {
            let self = this;
            window.console.log("ScannerInterfaceWeb");
            let configuration = {
                onComplete: function (data) {
                    self.acquireResponse = data;
                }, //function called when scan complete
                viewButton: $(".k1ViewBtn"), //This is optional. Specify a element that when clicked will view scanned document
                fileUploadURL: document.location.origin + '/Home/UploadFile', //This is the service that the scanned document will be uploaded to when complete
                clientID: "" + Date.now(), //This is a way to identify the user who is scanning.  It should be unique per user.  Session ID could be used if no user logged in
                setupFile: document.location.origin + '/Home/DownloadSetup', //location of the installation file if service doesn't yet exist
                interfacePath: document.location.origin + "/interface.html", // This is optional if your application lives under a subdomain.
                scannerInterface: K1WebTwain.Options.ScannerInterface.Web,
                scanButton: $("#scanbtn"), // the scan button
            };

            K1WebTwain.Configure(configuration).then(x => {
                window.console.log(x);
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
