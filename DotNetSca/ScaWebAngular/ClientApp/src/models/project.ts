import {Report} from "./report";

export interface Project {
  id: number;
  name: string;
  description: string;
  reports: Report[];
}
