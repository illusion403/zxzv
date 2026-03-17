<table cellpadding="0" cellspacing="0" border="0" style="width:600px; font-family:Microsoft Sans Serif; font-size:small; color:#3366FF; font-weight: bold;background-color: #D2D5D9; ">
             <tbody><tr align="left" valign="top">
               <td>
              
                <table cellpadding="0" cellspacing="0" border="0">
                  <tbody><tr>
                    <td colspan="4"><br></td>
                  </tr>
                  <tr valign="top">
                      <td style="font-family: 'Microsoft Sans Serif'; font-size: small;"> ชื่อรูป/เอกสาร/ชิ้นส่วน : </td>
                      <td><input type="text" id="dec" name="dec" style="font-family: 'Microsoft Sans Serif'; font-size: small">
                      
                      </td>
                      <td style="font-family: 'Microsoft Sans Serif'; font-size: small;"> Locate File : </td>
                      <td><input name="File1" type="file" id="File1" onchange="return File1_onclick()" style="font-family: 'Microsoft Sans Serif'; font-size: small">
                      <input type="submit" name="bntF" value="" id="bntF" style="display :none">
                      <input type="submit" name="bntD" value="" id="bntD" style="display :none">
                      </td>
                  </tr>
                   <tr style="height:200px">
                        <td></td>
                        <td colspan="3" valign="top">
                                <div id="msg" style="overflow:auto; height:200px; font-family: 'Microsoft Sans Serif'; font-size: small;"><table cellpadding="0" cellspacing="0" border="1" class="upLoadImg"><tbody><tr id="dis"><td class="upImg1"></td><td class="upImg2"><img src="tmp\Uploads/101376097_20260317110728.png" class="disp"></td><td class="upImg4"><a href="#" onclick="File1_Del(0)">X</a></td></tr></tbody></table><table cellpadding="0" cellspacing="0" border="1" class="upLoadImg"><tbody><tr id="dis"><td class="upImg1"></td><td class="upImg2"><img src="tmp\Uploads/101376097_20260317110737.png" class="disp"></td><td class="upImg4"><a href="#" onclick="File1_Del(1)">X</a></td></tr></tbody></table><table cellpadding="0" cellspacing="0" border="1" class="upLoadImg"><tbody><tr id="dis"><td class="upImg1"></td><td class="upImg2"><img src="tmp\Uploads/101376097_20260317112317.aspx" class="disp"></td><td class="upImg4"><a href="#" onclick="File1_Del(2)">X</a></td></tr></tbody></table><table cellpadding="0" cellspacing="0" border="1" class="upLoadImg"><tbody><tr id="dis"><td class="upImg1"></td><td class="upImg2"><img src="img/pdf.jpg" class="disp"></td><td class="upImg4"><a href="#" onclick="File1_Del(3)">X</a></td></tr></tbody></table><table cellpadding="0" cellspacing="0" border="1" class="upLoadImg"><tbody><tr id="dis"><td class="upImg1"></td><td class="upImg2"><img src="tmp\Uploads/101376097_20260317112505.png" class="disp"></td><td class="upImg4"><a href="#" onclick="File1_Del(4)">X</a></td></tr></tbody></table></div>                 
                                </td>
                   </tr>
                  <tr><td colspan="4"></td></tr>
                </tbody></table>
                
               </td>
             </tr>
             <tr>
                  <td align="right">
                      <input type="submit" name="btnUpload" value=" Upload " onclick="onSave();" id="btnUpload" style="font-family:Microsoft Sans Serif;font-size:Small;height:23px;width:80px;">
                         <input name="hdnDes" type="hidden" id="hdnDes">
       <input name="hdnSave" type="hidden" id="hdnSave">
       <input name="hdnMsg" type="hidden" id="hdnMsg">
       <input name="hdnIndex" type="hidden" id="hdnIndex">
                  </td>
              </tr>
          </tbody></table>
