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
        this.departments = response.data || [];
        this.emitInitialSelection();
    },

    methods: {
        emitInitialSelection() {
            if (!this.departments.length) return;

            const selectedDepartment = this.departments.find(
                x => x.departmentCode === this.currentDepartmentCode
            ) || this.departments[0];

            this.$emit('page-change', {
                component: this.pages[selectedDepartment.departmentCode] || this.pages[this.currentDepartmentCode],
                departmentCode: selectedDepartment.departmentCode,
                departmentId: selectedDepartment.departmentId
            });
        }
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
