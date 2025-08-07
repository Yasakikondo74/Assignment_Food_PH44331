<template>
  <div class="container-fluid">
    <div class="row">
      <div class="col-12">
        <h3 class="mt-4 mb-3">Account List (Admin)</h3>
        <div class="card shadow-sm">
          <div class="card-body">
            <table class="table table-striped table-hover">
              <thead>
                <tr>
                  <th scope="col">ID</th>
                  <th scope="col">Username</th>
                  <th scope="col">Email</th>
                  <th scope="col">Role</th>
                  <th scope="col">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="x in account_data" :key="x.id">
                  <td class="fw-bold">{{ x.id }}</td>
                  <td>
                    <input
                      v-model="editedRows[x.id].accUsername"
                      class="form-control"
                      @input="markChanged(x.id)"
                    />
                  </td>
                  <td>
                    <input
                      type="email"
                      v-model="editedRows[x.id].email"
                      class="form-control"
                      @input="markChanged(x.id)"
                    />
                  </td>
                  <td>
                    <select
                      v-model="editedRows[x.id].accRole"
                      class="form-control"
                      @change="markChanged(x.id)"
                    >
                      <option value="admin">admin</option>
                      <option value="client">client</option>
                    </select>
                  </td>
                  <td>
                    <div v-if="hasChanged(x.id)">
                      <button class="btn btn-success btn-sm me-1" @click="saveChanges(x.id)">
                        Save
                      </button>
                      <button class="btn btn-secondary btn-sm" @click="discardChanges(x.id)">
                        Discard
                      </button>
                    </div>
                    <div v-else>
                      <button class="btn btn-danger btn-sm me-1" @click="remove(x.id)">
                        Delete
                      </button>
                      <RouterLink
                        :to="`/admin/account/detail/${x.id}`"
                        class="btn btn-info btn-sm text-white"
                      >
                        Detail
                      </RouterLink>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="card-footer d-flex justify-content-end">
            <RouterLink to="/admin/account/add" class="btn btn-primary">
              Add New Account
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, inject, onMounted } from 'vue';
import { RouterLink } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const url = inject('url');
const account_data = ref([]);

const originalRows = reactive({});
const editedRows = reactive({});
const dirtyFlags = reactive({});

async function callAPI() {
  try {
    const res = await axios.get(url + '/accounts');
    account_data.value = res.data;

    res.data.forEach((item) => {
      originalRows[item.id] = { ...item };
      editedRows[item.id] = { ...item };
      dirtyFlags[item.id] = false;
    });
  } catch (error) {
    console.error('Failed to fetch account data:', error);
  }
}

function markChanged(id) {
  dirtyFlags[id] = hasChanged(id);
}

function hasChanged(id) {
  const orig = originalRows[id];
  const edited = editedRows[id];
  if (!orig || !edited) return false;
  return (
    orig.accUsername !== edited.accUsername ||
    orig.email !== edited.email ||
    orig.accRole !== edited.accRole
  );
}

async function saveChanges(id) {
  if (!id) {
    console.error('ID is invalid, cannot save changes.');
    alert('Invalid item ID. Cannot save.');
    return;
  }
  try {
    await axios.put(`${url}/account/${id}`, editedRows[id]);
    alert('Successfully updated');
    originalRows[id] = { ...editedRows[id] };
    dirtyFlags[id] = false;
    await callAPI(); // Refresh the data to be safe
  } catch (error) {
    console.error('Failed to update account:', error);
    alert('Failed to update account.');
  }
}

function discardChanges(id) {
  if (originalRows[id]) {
    editedRows[id] = { ...originalRows[id] };
    dirtyFlags[id] = false;
  }
}

async function remove(id) {
  if (confirm('Are you sure you want to delete this account?')) {
    try {
      await axios.delete(`${url}/account/${id}`);
      alert('Successfully Deleted');
      await callAPI(); // Refresh the data after deletion
    } catch (error) {
      console.error('Failed to delete account:', error);
      alert('Failed to delete account.');
    }
  }
}

onMounted(() => {
  callAPI();
});
</script>