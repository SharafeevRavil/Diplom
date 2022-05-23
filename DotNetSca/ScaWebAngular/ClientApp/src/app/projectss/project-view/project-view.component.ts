import {Component, Inject, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Project} from "../../../models/project";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-project-view',
  templateUrl: './project-view.component.html',
  styleUrls: ['./project-view.component.css']
})
export class ProjectViewComponent implements OnInit {
  private projectId!: number;

  public project!: Project;

  constructor(private actRoute: ActivatedRoute, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
              private router: Router) {
  }

  ngOnInit() {
    this.actRoute.paramMap.subscribe((params) => {
      this.projectId = Number(params.get('id')!);

      this.load();
    });
  }

  private load() {
    this.http.get<Project>(this.baseUrl + 'api/projects/' + this.projectId).subscribe(result => {
      this.project = result;
    }, error => {
      console.error(error)
    });
  }

  open(id: number) {
    this.router.navigateByUrl('/projects/' + this.projectId + '/reports/' + id)
  }
}
