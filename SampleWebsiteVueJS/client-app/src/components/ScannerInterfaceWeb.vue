<template>
    <div id="k1interface-visible" :class="{ 'show': isDisplayUI, 'hide': !isDisplayUI }">
        <div><label class="scanning-label">Scan Document:</label></div>
        <div class="input-group">
            <div class="input-group-btn">
                <button id="scanbtn" type="button" class="btn btn-primary" aria-label="Bold">
                    <span>Scan</span>
                </button>
            </div>
        </div>
    </div>
</template>

<script>
    import $ from 'jquery'
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss_obfuscated.js'

    export default {
        name: 'ScannerInterfaceWeb',
        data: function () {
            return {
                isDisplayUI: false
            }
        },
        methods: {},
        props: {},
        mounted: function () {
            let self = this;
            let configuration = {
                onComplete: function (response) {
                    self.$parent.completeAcquire({
                        acquireResponse: JSON.stringify(response.uploadResponse, null, 4),
                        acquireError: '',
                    });
                }, //function called when scan complete
                 viewButton: null, //This is optional. Specify a element that when clicked will view scanned document
                fileUploadURL: document.location.origin + '/Home/UploadFile', //This is the service that the scanned document will be uploaded to when complete
                clientID: "" + Date.now(), //This is a way to identify the user who is scanning.  It should be unique per user.  Session ID could be used if no user logged in
                setupFile: document.location.origin + '/Home/DownloadSetup', //location of the installation file if service doesn't yet exist
                interfacePath: document.location.origin + "/interface.html", // This is optional if your application lives under a subdomain.
                scannerInterface: K1WebTwain.Options.ScannerInterface.Web,
                scanButton: $("#scanbtn"), // the scan button
            };

            K1WebTwain.Configure(configuration).then(() => {
                this.isDisplayUI = false;

                K1WebTwain.ResetService().then(function () {
                    setTimeout(() => {
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
