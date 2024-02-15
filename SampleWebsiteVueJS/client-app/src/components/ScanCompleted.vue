<template>
    <div>
        <div class="alert alert-success" id="uploadResponseDiv">
            <div>
                <label class="scanning-label">{{isUpload? "Response from file upload route:" : "File Info:"}}</label>
            </div>
            <pre id="uploadResponseOutput">{{msg}}</pre>
             <div className="input-group-btn fileview">
                 <button id="viewBtn2" type="button" class="btn btn-default k1ViewBtn" @click="downloadDocument">{{isUpload ? "View" : "Download"}}</button>
            </div>
        </div>
    </div>
</template>

<script>
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss.js'

    export default {
        name: 'ScanCompleted',
        methods: {
            downloadDocument: function() {
                K1WebTwain.SPADownloadDocument();
            }
        },
        props: {
            msg: String,
            saveToType: Number
        },
        data: function () {
            return {
                isUpload: false
            }
        },
        mounted: function () {
            this.isUpload = this.saveToType === K1WebTwain.Options.SaveToType.Upload;
        }
    }
</script>