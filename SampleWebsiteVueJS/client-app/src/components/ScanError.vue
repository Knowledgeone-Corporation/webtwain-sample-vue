<template>
    <div>
        <div class="alert alert-danger" id="errorMessageDiv">
            <div>
                <label class="scanning-label">Error</label>
            </div>
            <pre id="errorMessageOutput">{{msg}}</pre>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'ScanError',
        props: {
            msg: String
        },
    }
</script>