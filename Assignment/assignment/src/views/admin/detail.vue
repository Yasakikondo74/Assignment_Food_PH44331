<template>
    <form class="form-control">
        <RouterLink to="/admin/food/list" class="btn btn-warning">Back</RouterLink><br>
        <label><span>Title</span><input type="text" class="input-group-text" v-model="food_data.foodName" disabled></label>
        <label><span>Artist</span><input type="text" class="input-group-text" v-model="food_data.available" disabled></label>
        <label><span>Album</span><input type="text" class="input-group-text" v-model="food_data.quantity" disabled></label>
        <label><span>Full Cover Link</span><input type="text" class="input-group-text" v-model="food_data.cost" disabled></label><br><br>
        <button class="btn btn-danger" @click="remove(food_data.id)">Delete</button> &nbsp;
        <RouterLink :to="'/admin/food/update/' + id" class="btn btn-warning">Edit</RouterLink>
    </form>
</template>
<script setup>
import { ref, inject, onMounted } from 'vue'
import { RouterLink, useRouter, useRoute } from 'vue-router'
import axios from 'axios'
const r = useRoute()
const id = r.params.id
const rr = useRouter()
const url = inject("url")
const food_data = ref({
    foodName: "",
    available: false,
    quantity: 0,
    cost: 0
})

async function callAPI(){
    let res = await axios.get(url + '/food/' + id)
    food_data.value = res.data
}

onMounted( async () => {
    callAPI()
})

async function remove() {
    if(confirm("Are you sure you want to change this?")){
        await axios.delete(url + "/food/" + id)
        alert("delete successfully")
        rr.push("/admin/food/list")
    }
}

</script>

<style lang="sass" scope>
span
    font-size: 150%
</style>