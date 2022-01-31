<template>
    <div>
        <div class="alert alert-success" id="uploadResponseDiv">
            <div>
                <label>Response from file upload route:</label>
            </div>
            <pre id="uploadResponseOutput">{{msg}}</pre>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'ScanCompleted',
        props: {
            msg: String
        },
    }
</script>