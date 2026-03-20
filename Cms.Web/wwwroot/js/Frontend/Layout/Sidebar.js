import { getAccessibleDepartments } from '../api/departmentApi.js';

export const Sidebar = {

    props: {
        currentPage: Object,
        pages: Object,
        currentDepartmentCode: String
    },

    emits: ['page-change'],

    data() {
        return {
            departments: []
        };
    },

    async mounted() {
        const response = await getAccessibleDepartments();
        this.departments = response.data;
    },

    template: `
        <nav class="frontend-sidebar">

            <div
                v-for="department in departments"
                :key="department.departmentCode"
                class="frontend-sidebar-item"
                :class="{ active: currentDepartmentCode === department.departmentCode }"
                @click="$emit('page-change', {
                    component: pages[department.departmentCode],
                    departmentCode: department.departmentCode,
                    departmentId: department.departmentId
                })"
            >
                {{ department.departmentName }}
            </div>

        </nav>
    `
};
