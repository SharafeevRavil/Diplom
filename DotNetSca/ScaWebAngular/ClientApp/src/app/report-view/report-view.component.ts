import {Component, Inject, OnInit} from '@angular/core';
import {Project} from "../../models/project";
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {ReportWithData} from "../../models/report-with-data";

@Component({
  selector: 'app-report-view',
  templateUrl: './report-view.component.html',
  styleUrls: ['./report-view.component.css']
})
export class ReportViewComponent implements OnInit {
  private projectId!: number;
  private reportId!: number;

  public report!: ReportWithData;

  constructor(private actRoute: ActivatedRoute, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
              private router: Router) {
  }

  ngOnInit() {
    this.actRoute.paramMap.subscribe((params) => {
      this.projectId = Number(params.get('projectId')!);
      this.reportId = Number(params.get('reportId')!);

      this.load();
    });
  }

  private load() {
    this.http.get<ReportWithData>(this.baseUrl + 'api/projects/' + this.projectId + '/reports/' + this.reportId).subscribe(result => {
      this.report = result;
    }, error => {
      console.error(error)
    });
  }

  back() {
    this.router.navigateByUrl('/projects/' + this.projectId)
  }
}
