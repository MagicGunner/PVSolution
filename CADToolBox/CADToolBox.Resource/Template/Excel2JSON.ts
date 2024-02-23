
/**@米可爱分享
* 付费咨询 https://work.weixin.qq.com/kfid/kfce4e306b8e3c2d528
* 专栏 https://mp.weixin.qq.com/mp/appmsgalbum?__biz=Mzg3MzcwNzcxNA==&action=getalbum&album_id=2363274956890800129#wechat_redirect
*/
$("#exportToJson").click(() => exportToJson());
/** 导出选中区域到JSON文件
*/
async function exportToJson(){
  Excel.run(async (context) => {
    var range: Excel.Range = context.workbook.getSelectedRange();
    range.load(["values"]);
    await context.sync();
    var values: any[][] = range.values;
    console.log(values);
    // 获取第一行作为Json字段
    var headers: any[] = values[0];
    // 将1-n行作为值写入Json对象
    var target:any[] = [];
    for(let i:number = 1; i < values.length; i ++) {
      let row: any = {};
      for (let j:number = 0; j < headers.length; j++) {
        row[headers[j]] = values[i][j];
      }
      target.push(row);
    }
    console.log(JSON.stringify(target));
    await writeToFile("关注米可爱分享微信公众号.json", JSON.stringify(target), "application/json");
  });
}
async function writeToFile(fileName, fileContent, fileType){
    var a = document.createElement("a");
    var file= new Blob([fileContent], {type: fileType});
    a.href = URL.createObjectURL(file);
    a.download = fileName;
    a.click();
}