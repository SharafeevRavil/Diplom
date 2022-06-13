export interface ReportWithData {
  id: number;

  initiator: string;
  createdDate: Date;
  isFilledByServer: boolean;

  explicitPackages: Package[];
  generatedSignatures: Signature[];
}

export interface Package {
  id: number;

  description: string;
  packageId: string;
  packageVersion: string;
  vulnerabilities: Vulnerability[];
}

export interface Vulnerability {
  cve : string;
  cwe : string;
  description : string;
  reference : string;
  title : string;
  cvssScore : string;
  cvssVector : string;
}

export interface Signature {
  id: number;
  hash: string;
  description: string;
  packages: Package[];
}
