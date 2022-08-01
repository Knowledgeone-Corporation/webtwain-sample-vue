<template>
    <div>
        <div class="alert alert-success" id="uploadResponseDiv">
            <div>
                <label class="scanning-label">Response from file upload route:</label>
            </div>
            <pre id="uploadResponseOutput">{{msg}}</pre>
             <div className="input-group-btn fileview">
                <button id="viewBtn2" type="button" class="btn btn-default k1ViewBtn" @click="downloadDocument">View</button>
            </div>
        </div>
    </div>
</template>

<script>
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss_obfuscated.js'

    export default {
        name: 'ScanCompleted',
        methods: {
            downloadDocument: function() {
                K1WebTwain.SPADownloadDocument();
            }
        },
        props: {
            msg: String
        },
    }
</script>