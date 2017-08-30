using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Net;

namespace DemoXMLCScharp
{
    public partial class frmPhieuNhapXuat : Form
    {
        string path = "../../Data/HANGHOA.xml";
        XmlDocument doc;
        XmlNodeList dsNhanVien, dsCTPNX;
        XmlNode PNX;
        public frmPhieuNhapXuat()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                doc = new XmlDocument();
                doc.Load(path);
                dsNhanVien = doc.SelectNodes("//nhanvien");
                
                //Hien thi cboLoaiPhieu
                cboLoaiPhieu.Items.Add("Nhập");
                cboLoaiPhieu.Items.Add("Xuất");
                //Hien thi danh sach nhan vien
                foreach(XmlNode n in dsNhanVien)
                {
                   cboNhanVien.Items.Add(n.ChildNodes.Item(1).InnerText);
                }
                PNX = doc.SelectNodes("//phieunx").Item(0);
                hienThiPhieuNX();
                hienThiChiTietPNX();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(PNX.NextSibling !=null)
            {
                PNX = PNX.NextSibling;
                hienThiPhieuNX();
                hienThiChiTietPNX();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if(PNX.PreviousSibling !=null)
            {
                PNX = PNX.PreviousSibling;
                hienThiPhieuNX();
                hienThiChiTietPNX();
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //Kiem tra SoPhieu ton tai
            XmlNodeList nodes = doc.SelectNodes("//phieunx[SoPhieu='" + txtSoPhieu.Text + "']");
            if(nodes.Count>0)
            {
                MessageBox.Show("Số phiếu đã tồn tại !!!");
                return;
            };


            //Thuc hien them
            try
            {
                XmlElement phieu = doc.CreateElement("phieunx");
                XmlElement soPhieu = doc.CreateElement("SoPhieu");
                XmlText text = doc.CreateTextNode(txtSoPhieu.Text);
                soPhieu.AppendChild(text);
                phieu.AppendChild(soPhieu);
                XmlElement loaiPhieu = doc.CreateElement("LoaiPhieu");
                text = doc.CreateTextNode(cboLoaiPhieu.SelectedItem.ToString());
                loaiPhieu.AppendChild(text);
                phieu.AppendChild(loaiPhieu);
                XmlElement ngayLapPhieu = doc.CreateElement("NgayLapPhieu");
                text = doc.CreateTextNode(txtNgayLapPhieu.Text);
                ngayLapPhieu.AppendChild(text);
                phieu.AppendChild(ngayLapPhieu);
                XmlElement maNV = doc.CreateElement("MaNV");
                text = doc.CreateTextNode(dsNhanVien.Item(cboNhanVien.SelectedIndex).FirstChild.InnerText);
                maNV.AppendChild(text);
                phieu.AppendChild(maNV);
                XmlElement triGia = doc.CreateElement("TriGia");
                text = doc.CreateTextNode("0");
                triGia.AppendChild(text);
                phieu.AppendChild(triGia);
                XmlNode parent = PNX.ParentNode;
                parent.AppendChild(phieu);
                doc.Save(path);
                PNX = phieu;
                hienThiPhieuNX();
                hienThiChiTietPNX();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            XmlNode t = PNX.PreviousSibling;
            XmlNode parent = PNX.ParentNode;
            if(dsCTPNX.Count > 0)
            {
                MessageBox.Show("Không thể xóa do có chi tiết PNX!!!", "Lỗi xóa", MessageBoxButtons.OK ,MessageBoxIcon.Error);
            }
            else
            {
                parent.RemoveChild(PNX);
                doc.Save(path);
                PNX = t;
                hienThiPhieuNX();
                hienThiChiTietPNX();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            PNX.ChildNodes.Item(0).InnerText = txtSoPhieu.Text;
            PNX.ChildNodes.Item(1).InnerText = cboLoaiPhieu.SelectedItem.ToString();
            PNX.ChildNodes.Item(2).InnerText = txtNgayLapPhieu.Text;
            PNX.ChildNodes.Item(3).InnerText = dsNhanVien.Item(cboNhanVien.SelectedIndex).FirstChild.InnerText;
            PNX.ChildNodes.Item(4).InnerText = txtTriGia.Text;
            doc.Save(path);
        }

        private void hienThiPhieuNX()
        {
            if(PNX !=null)
            {
                txtSoPhieu.Text = PNX.ChildNodes.Item(0).InnerText;
                cboLoaiPhieu.SelectedItem = PNX.ChildNodes.Item(1).InnerText;
                txtNgayLapPhieu.Text = PNX.ChildNodes.Item(2).InnerText;
                String maNV = PNX.ChildNodes.Item(3).InnerText;

                foreach(XmlNode n in dsNhanVien)
                {
                    String ma = n.ChildNodes.Item(0).InnerText;
                    if (maNV == ma)
                    {
                        cboNhanVien.SelectedItem = n.ChildNodes.Item(1).InnerText;
                        break;
                    }
                }

                txtTriGia.Text = PNX.ChildNodes.Item(4).InnerText;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hienThiChiTietPNX()
        {
            int sum = 0;
            if (PNX !=null)
            {
                dgvCTPNX.Rows.Clear();
                String soPhieu = PNX.FirstChild.InnerText;
                dsCTPNX = doc.SelectNodes("//ctphieunx[SoPhieu='"+ soPhieu + "']");
                foreach(XmlNode n in dsCTPNX)
                {
                    String maHH = n.ChildNodes.Item(1).InnerText;
                    String tenHH = doc.SelectSingleNode("//hanghoa[MaHH='" + maHH + "']/TenHH").InnerText;
                    int soLuong = int.Parse( n.ChildNodes.Item(2).InnerText);
                    int donGia = int.Parse(n.ChildNodes.Item(3).InnerText);
                    int thanhTien = soLuong * donGia;
                    dgvCTPNX.Rows.Add(tenHH, soLuong,donGia, thanhTien);
                    sum += thanhTien;
                }
                txtTriGia.Text = sum+"";
            }
        }
    }
}
