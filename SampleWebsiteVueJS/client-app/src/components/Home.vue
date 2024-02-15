<template>
    <div>
        <div><h1>Scan Demo Page : Vue</h1></div>
        <div class="alert alert-info alert-dismissible show" role="alert">
            <strong>Note: </strong>The demonstration service has the following limitations;
            - generated documents contain a watermark,
            - multi-page documents are limited to 10 pages,
            - OCR is limited to the first page of the document.
            These limitations are not present in the licensed product.
        </div>


        <div className="form-group">
            <label class="scanning-label">Interface Option</label>
            <div>
                <select v-model="selectedOption" class="form-control" @change="onInterfaceChange($event)">
                    <option value="-1">Please select...</option>
                    <option value="0">Hidden : Not using Webtwain or Native UI</option>
                    <option value="1">Visible : Uses Webtwain and Native UI</option>
                    <option value="2">Web : only uses Webtwain UI</option>
                    <option value="3">Desktop : only uses Native scanner UI</option>
                </select>
            </div>
        </div>

        <ScannerInterfaceHidden v-if="selectedOption == 0"></ScannerInterfaceHidden>
        <ScannerInterfaceVisible v-if="selectedOption == 1"></ScannerInterfaceVisible>
        <ScannerInterfaceWeb v-if="selectedOption == 2"></ScannerInterfaceWeb>
        <ScannerInterfaceDesktop v-if="selectedOption == 3"></ScannerInterfaceDesktop>

        <ScanCompleted v-if="acquireResponse" :msg="acquireResponse" :saveToType="saveToType"></ScanCompleted>
        <ScanError v-if="acquireError" :msg="acquireError"></ScanError>
    </div>
</template>

<script>
    import ScanCompleted from './ScanCompleted'
    import ScanError from './ScanError'
    import ScannerInterfaceDesktop from './ScannerInterfaceDesktop.vue'
    import ScannerInterfaceHidden from './ScannerInterfaceHidden.vue'
    import ScannerInterfaceVisible from './ScannerInterfaceVisible.vue'
    import ScannerInterfaceWeb from './ScannerInterfaceWeb.vue'
    import { K1WebTwain } from '../lib/k1scanservice/js/k1ss.js'

    import '../lib/k1scanservice/css/k1ss.min.css';

    export default {
        name: 'Home',
        data: function () {
            return {
                selectedOption: -1,
                acquireResponse: '',
                acquireError: '',
                saveToType: K1WebTwain.Options.SaveToType.Upload
            }
        },
        methods: {
            onInterfaceChange: function ($event) {
                this.selectedOption = $event.target.value;
                this.acquireResponse = '';
                this.acquireError = '';
                this.saveToType = K1WebTwain.Options.SaveToType.Upload;
            },
            completeAcquire: function ($event) {
                this.selectedOption = -1;
                this.acquireResponse = $event.acquireResponse;
                this.acquireError = $event.acquireError;
                this.saveToType = $event.saveToType;
            }     
        },
        components: {
            ScanCompleted,
            ScanError,
            ScannerInterfaceDesktop,
            ScannerInterfaceHidden,
            ScannerInterfaceVisible,
            ScannerInterfaceWeb
        },
        props: {
            msg: String,
        },
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
