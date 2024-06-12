<template>
    <div id="app" class="container">
        <Home />
    </div>
</template>

<script>
import Home from './components/Home.vue'

export default {
    name: 'app',
    components: {
        Home,
    }
}
</script>

<style>
#app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #2c3e50;
    margin-top: 30px;
}

.input-group-addon {
    padding: 6px 12px;
    font-size: 14px;
    font-weight: 400;
    line-height: 1;
    color: #555;
    text-align: center;
    background-color: #eee;
    border: 1px solid #ccc;
}

.btn-default {
    color: #333;
    background-color: #fff;
    border-color: #ccc;
}

.btn:hover {
    color: #333;
    text-decoration: none;
}

.scanning-label {
    font-weight: 700;
}

.alert>pre {
    display: block;
    padding: 9.5px;
    margin: 0 0 10px;
    font-size: 13px;
    line-height: 1.42857143;
    color: #333333;
    word-break: break-all;
    word-wrap: break-word;
    background-color: #f5f5f5;
    border: 1px solid #ccc;
    border-radius: 4px;
}

.section-disabled {
    opacity: 0.5;
    pointer-events: none;
}

.filetype-restriction {
    color: #337ab7;
    font-size: 13px;
    margin-left: 4px;
}

.finalize-section div {
    margin-top: 6px;
}
</style>
