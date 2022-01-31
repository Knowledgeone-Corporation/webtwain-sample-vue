<template>
    <div>
        <div><h1>Scan Demo Page</h1></div>
        <div class="alert alert-info alert-dismissible show" role="alert">
            <strong>Note: </strong>The demonstration service has the following limitations;
            - generated documents contain a watermark,
            - multi-page documents are limited to 10 pages,
            - OCR is limited to the first page of the document.
            These limitations are not present in the licensed product.
        </div>


        <div className="form-group">
            <label>Interface Option</label>
            <div>
                <select v-model="selectedOption" class="form-control">
                    <option value="-1">Please select...</option>
                    <option value="0">Hidden</option>
                    <option value="1">Visible</option>
                    <option value="2">Web</option>
                    <option value="3">Desktop</option>
                </select>
            </div>
        </div>

        <ScannerInterfaceHidden v-if="selectedOption == 0"></ScannerInterfaceHidden>
        <ScannerInterfaceVisible v-if="selectedOption == 1"></ScannerInterfaceVisible>
        <ScannerInterfaceWeb v-if="selectedOption == 2"></ScannerInterfaceWeb>
        <ScannerInterfaceDesktop v-if="selectedOption == 3"></ScannerInterfaceDesktop>
    </div>
</template>

<script>
    import ScannerInterfaceDesktop from './ScannerInterfaceDesktop.vue'
    import ScannerInterfaceHidden from './ScannerInterfaceHidden.vue'
    import ScannerInterfaceVisible from './ScannerInterfaceVisible.vue'
    import ScannerInterfaceWeb from './ScannerInterfaceWeb.vue'

    export default {
        name: 'Home',
        data: function () {
            return {
                selectedOption: -1,
            }
        },
        components: {
            ScannerInterfaceDesktop,
            ScannerInterfaceHidden,
            ScannerInterfaceVisible,
            ScannerInterfaceWeb
        },
        props: {
            msg: String
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
