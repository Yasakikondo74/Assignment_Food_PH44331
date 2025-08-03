<template>
  <table class="table table-striped table-hover">
    <thead>
      <tr>
        <th scope="col">ID</th>
        <th scope="col">Name</th>
        <th scope="col">Available</th>
        <th scope="col">Quantity</th>
        <th scope="col">Cost</th>
        <th scope="col">Functions</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="x in food_data" :key="x.id">
        <td class="fw-bold">{{ x.id }}</td>

        <!-- Always-editable inputs -->
        <td>
          <input
            v-model="editedRows[x.id].foodName"
            class="form-control"
            @input="markChanged(x.id)"
          />
        </td>

        <td>
          <select
            v-model="editedRows[x.id].available"
            class="form-control"
            @change="markChanged(x.id)"
            disabled
          >
            <option value="true">true</option>
            <option value="false">false</option>
          </select>
        </td>

        <td>
          <input
            type="number"
            v-model.number="editedRows[x.id].quantity"
            class="form-control"
            @input="markChanged(x.id)"
          />
        </td>

        <td>
          <div class="d-flex align-items-center">
            <input
              type="number"
              v-model.number="editedRows[x.id].cost"
              class="form-control me-1"
              style="max-width: 100px;"
              @input="markChanged(x.id)"
            />
            <span>$</span>
          </div>
        </td>

        <!-- Actions -->
        <td>
          <template v-if="hasChanged(x.id)">
            <button class="btn btn-success btn-sm" @click="saveChanges(x.id)">
              Save
            </button>
            &nbsp;
            <button class="btn btn-secondary btn-sm" @click="discardChanges(x.id)">
              Discard
            </button>
          </template>
          <template v-else>
            <button class="btn btn-danger btn-sm" @click="remove(x.id)">
              Delete
            </button>
            &nbsp;
            <RouterLink :to="`/admin/detail/${x.id}`" class="btn btn-secondary btn-sm">
              Show More Detail
            </RouterLink>
          </template>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script setup>
import { ref, reactive, inject, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import axios from 'axios'

const url = inject('url')
const food_data = ref([])

const originalRows = reactive({})
const editedRows = reactive({})
const dirtyFlags = reactive({})

async function callAPI() {
  try {
    const res = await axios.get(url + '/food')
    food_data.value = res.data

    res.data.forEach(item => {
      originalRows[item.id] = { ...item } 
      editedRows[item.id] = { ...item } 
      dirtyFlags[item.id] = false
    });
  } catch (error) {
    console.error('Failed to fetch food data:', error)
  }
}

function markChanged(id) {
  dirtyFlags[id] = hasChanged(id)
}

function hasChanged(id) {
  const orig = originalRows[id]
  const edited = editedRows[id]
  if (!orig || !edited) return false;
  return (
    orig.foodName !== edited.foodName ||
    String(orig.available) !== String(edited.available) ||
    Number(orig.quantity) !== Number(edited.quantity) ||
    Number(orig.cost) !== Number(edited.cost)
  );
}

async function saveChanges(id) {
  try {
    await axios.put(`${url}/food/${id}`, editedRows[id])
    alert('Successfully updated');
    originalRows[id] = { ...editedRows[id] }
    dirtyFlags[id] = false
    await callAPI();
  } catch (error) {
    console.error('Failed to update item:', error)
    alert('Failed to update item.')
  }
}

function discardChanges(id) {
  if (originalRows[id]) {
    editedRows[id] = { ...originalRows[id] }
    dirtyFlags[id] = false
  }
}

async function remove(id) {
  if (confirm('Are you sure you want to delete this?')) {
    try {
      await axios.delete(`${url}/food/${id}`)
      alert('Successfully Deleted')
      await callAPI()
    } catch (error) {
      console.error('Failed to delete food item:', error)
      alert('Failed to delete item.')
    }
  }
}

onMounted(() => {
  callAPI();
});
</script>
