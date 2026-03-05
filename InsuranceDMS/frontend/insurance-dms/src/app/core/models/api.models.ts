export interface ApiResponse<T> {
  data: T;
  errors: string[];
  pagination?: PaginationMeta;
}

export interface PaginationMeta {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export enum AgencyTier {
  FMO = 1, MGA = 2, GA = 3, Agent = 4
}

export enum PersonnelType {
  Producer = 1, CSR = 2, Manager = 3, Principal = 4
}

export enum MergerStatus {
  Draft = 'Draft',
  Previewed = 'Previewed',
  Approved = 'Approved',
  Executing = 'Executing',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export interface Agency {
  id: number;
  agencyName: string;
  npn?: string;
  taxId?: string;
  agencyTier: AgencyTier;
  parentAgencyId?: number;
  parentAgencyName?: string;
  phone?: string;
  email?: string;
  website?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  stateCode?: string;
  zipCode?: string;
  county?: string;
  isActive: boolean;
  notes?: string;
  isMerged: boolean;
  mergedIntoId?: number;
  mergedAt?: string;
  createdAt: string;
  modifiedAt?: string;
}

export interface AgencySummary {
  id: number;
  agencyName: string;
  npn?: string;
  agencyTier: AgencyTier;
  isActive: boolean;
  isMerged: boolean;
  stateCode?: string;
  parentAgencyId?: number;
}

export interface Personnel {
  id: number;
  agencyId: number;
  agencyName: string;
  firstName: string;
  lastName: string;
  middleName?: string;
  email?: string;
  phone?: string;
  personnelType: PersonnelType;
  title?: string;
  hireDate?: string;
  terminationDate?: string;
  isActive: boolean;
  producer?: Producer;
  createdAt: string;
}

export interface Producer {
  id: number;
  personnelId: number;
  npn?: string;
  residentState?: string;
  ssnLast4?: string;
  dateOfBirth?: string;
  eoExpirationDate?: string;
  licenses?: License[];
  appointments?: Appointment[];
}

export interface License {
  id: number;
  producerId: number;
  stateCode: string;
  licenseTypeId: number;
  licenseTypeCode?: string;
  licenseNumber: string;
  issueDate?: string;
  expirationDate?: string;
  renewalDate?: string;
  isActive: boolean;
}

export interface Appointment {
  id: number;
  producerId: number;
  carrierId: number;
  carrierName?: string;
  stateCode: string;
  appointmentDate?: string;
  terminationDate?: string;
  appointmentStatus: string;
}

export interface Merger {
  id: number;
  survivingAgencyId: number;
  survivingAgencyName?: string;
  status: MergerStatus;
  initiatedBy?: string;
  initiatedAt: string;
  executedAt?: string;
  executedBy?: string;
  notes?: string;
  participants: MergerParticipant[];
}

export interface MergerParticipant {
  id: number;
  absorbedAgencyId: number;
  absorbedAgencyName?: string;
  personnelTransferred: number;
}

export interface MergerPreview {
  mergerId: number;
  survivingAgencyId: number;
  survivingAgencyName?: string;
  absorbedAgencies: AbsorbedAgencyPreview[];
  conflicts: string[];
  totalPersonnelToTransfer: number;
}

export interface AbsorbedAgencyPreview {
  agencyId: number;
  agencyName: string;
  personnelCount: number;
  producerCount: number;
  licenseCount: number;
  appointmentCount: number;
  duplicateNPNs: string[];
}

export interface EntityLineage {
  id: number;
  mergerId: number;
  entityType: string;
  sourceEntityId: number;
  targetEntityId?: number;
  sourceAgencyId: number;
  sourceAgencyName?: string;
  targetAgencyId: number;
  targetAgencyName?: string;
  action: string;
  recordedAt: string;
}

export interface Carrier {
  id: number;
  carrierName: string;
  naic?: string;
  isActive: boolean;
}

export interface State {
  stateCode: string;
  stateName: string;
}

export interface LicenseType {
  licenseTypeId: number;
  code: string;
  description: string;
}
