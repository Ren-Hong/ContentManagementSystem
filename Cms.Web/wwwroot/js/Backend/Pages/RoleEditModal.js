import { updateRole } from '../api/roleApi.js';
import { getPermissionLookups } from '../api/permissionApi.js';
import { getScopeLookups } from '../api/scopeApi.js';
import { RoleStatusText } from '../constants/roleStatus.js';

export const RoleEditModal = {
    props: {
        show: { // RoleManager 的 showEdit
            type: Boolean,
            required: true
        },
        role: { // RoleManager 的 selectedRole
            type: Object,
            default : null
        }
    },

    emits: ['close', 'updated'],

    data() {
        return {
            RoleStatusText, 

            modal: null,

            permissionLookups: [],
            scopeLookups: [],
            defaultScopeCode: 'Department',
            defaultScopeId: null,

            form: {
                roleName: '',
                status: '',
                permissionScopes: [] // PermissionId, ScopeId
            },

            submitting: false

        };
    },

    watch: {
        async show(val) {
            if (val) {
                await Promise.all([
                    this.loadPermissionLookups(),
                    this.loadScopeLookups()
                ]);

                this.fillForm();
                this.modal.show();
            } else {
                this.modal.hide();
            }
        }
    },

    mounted() {
        this.modal = new bootstrap.Modal(this.$refs.modal);

        // 使用者點 X / 點背景 關閉 modal
        this.$refs.modal.addEventListener('hidden.bs.modal', () => {
            this.$emit('close');
        });
    },

    methods: {
        async loadPermissionLookups() {
            try {
                const response = await getPermissionLookups();

                if (!response.success) {
                    alert(`錯誤代碼 -> ${response.errorCode}`);
                    return;
                }

                this.permissionLookups = response.data;
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 權限選單載入');
            }
        },

        async loadScopeLookups() {
            const response = await getScopeLookups();
            if (!response.success) {
                alert('Scope 選單載入失敗');
                return;
            }

            this.scopeLookups = response.data;

            const defaultScope = this.scopeLookups.find(
                scopeLookup => scopeLookup.scopeCode === this.defaultScopeCode
            );

            this.defaultScopeId = defaultScope?.scopeId ?? null;
        },

        onPermissionSelected(event) {
            const permissionId = event.target.value;
            if (!permissionId) return;

            if (!this.defaultScopeId) {
                alert('Scope 尚未載入');
                return;
            }

            const exists = this.form.permissionScopes.some(
                x => x.permissionId === permissionId
            );

            if (!exists) {
                this.form.permissionScopes.push({
                    permissionId,
                    scopeId: this.defaultScopeId
                });
            }

            event.target.value = '';
        },

        removePermission(permissionId) {
            this.form.permissionScopes = this.form.permissionScopes.filter(
                permissionScope => permissionScope.permissionId !== permissionId
            );
        },

        updateScope(permissionId, event) {
            const scopeId = event.target.value;
            const item = this.form.permissionScopes.find(x => x.permissionId === permissionId);
            if (item) item.scopeId = scopeId;
        },

        getPermissionName(permissionId) {
            const permissionLookup = this.permissionLookups.find(lookup => lookup.permissionId === permissionId);
            return permissionLookup ? permissionLookup.permissionName : permissionId;
        },

        fillForm() {
            this.form.roleName = this.role.roleName;
            this.form.status = this.role.status;

            this.form.permissionScopes = this.role.permissionScopes.flatMap(p =>
                p.scopes.map(s => ({
                    permissionId: p.permissionId,
                    scopeId: s.scopeId
                }))
            );
        },

        async submit() {
            if (this.form.permissionScopes.length === 0) {
                alert('請至少選擇一個權限');
                return;
            }

            this.submitting = true;

            try {
                const response = await updateRole(this.role.roleName, {
                    permissionScopes: this.form.permissionScopes,
                    status: this.form.status
                });

                if (!response.success) {
                    alert(`錯誤代碼 -> ${response.errorCode}`);
                    return;
                }

                this.$emit('updated');
            } catch (err) {
                console.error(err);
                alert('系統錯誤 -> 更新角色');
            } finally {
                this.submitting = false;
            }
        }
    },

    template: `
        <div class="modal fade" tabindex="-1" ref="modal">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-success">

                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title">編輯角色</h5>
                        <button type="button"
                                class="btn-close"
                                :disabled="submitting"
                                data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">
                        <div class="row">
                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>角色名稱 : </strong>
                                    <span>{{ role?.roleName }}</span>
                                </div>
                            </div>

                            <div class="col-6">
                                <div class="alert alert-success mb-3">
                                    <strong>角色狀態 : </strong>
                                    <span>{{ role ? RoleStatusText[role.status] : '' }}</span>
                                </div>
                            </div>
                        </div>

                        <div class="alert alert-success mb-3">

                            <div class="mb-2">
                                <label class="form-label">權限</label>

                                <select class="form-select"
                                        @change="onPermissionSelected($event)">
                                    <option value="">請選擇權限</option>
                                    <option v-for="permissionLookup in permissionLookups"
                                            :key="permissionLookup.permissionId"
                                            :value="permissionLookup.permissionId">
                                        {{ permissionLookup.permissionName }}
                                    </option>
                                </select>
                            </div>

                            <div class="d-flex flex-column gap-2">
                                <div v-for="ps in form.permissionScopes"
                                    :key="ps.permissionId"
                                    class="d-flex align-items-center gap-2">

                                    <span class="badge bg-primary">
                                        {{ getPermissionName(ps.permissionId) }}
                                    </span>

                                    <select class="form-select form-select-sm w-auto"
                                            :value="ps.scopeId"
                                            @change="updateScope(ps.permissionId, $event)">
                                        <option v-for="scopeLookup in scopeLookups"
                                                :key="scopeLookup.scopeId"
                                                :value="scopeLookup.scopeId">
                                            {{ scopeLookup.scopeName }}
                                        </option>
                                    </select>

                                    <button type="button"
                                            class="btn btn-danger btn-sm"
                                            @click="removePermission(ps.permissionId)">
                                        移除
                                    </button>
                                </div>
                            </div>

                        </div>

                        <div class="alert alert-success mb-3">
                            <label class="form-label">狀態</label>
                            <select class="form-select" v-model="form.status">
                                <option value="Enable">啟用</option>
                                <option value="Disabled">停用</option>
                            </select>
                        </div>

                        <div class="alert alert-success small">
                            變更將立即生效
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button class="btn btn-secondary btn-sm"
                                :disabled="submitting"
                                data-bs-dismiss="modal">
                            取消
                        </button>
                        <button class="btn btn-success btn-sm"
                                :disabled="submitting"
                                @click="submit">
                            {{ submitting ? '儲存中...' : '儲存' }}
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `
};
